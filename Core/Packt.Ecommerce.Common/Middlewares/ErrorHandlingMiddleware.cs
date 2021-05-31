using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Packt.Ecommerce.Common.Models;
using Packt.Ecommerce.Common.Options;
using Packt.Ecommerce.Common.Validator;

namespace Packt.Ecommerce.Common.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        private readonly ILogger logger;
        private readonly bool includeExceptionDetailsInResponse;

        public ErrorHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ErrorHandlingMiddleware> logger, IOptions<ApplicationSettings> applicationSettings)
        {
            NotNullValidator.ThrowIfNull(applicationSettings, nameof(applicationSettings));
            this.requestDelegate = requestDelegate;
            this.logger = logger;
            this.includeExceptionDetailsInResponse = applicationSettings.Value.IncludeExceptionStackInResponse;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if(this.requestDelegate != null)
                {
                    await this.requestDelegate.Invoke(context).ConfigureAwait(false);
                }
            }
            catch (Exception innerException)
            {
                this.logger.LogCritical(Constants.ErrorHandlingMiddlewareErrorCode, innerException, Constants.ErrorMiddlewareLog);

                ExceptionResponse currentException = new ExceptionResponse
                {
                    ErrorMessage = Constants.ErrorMiddlewareLog,
                    CorrelationIdentifier = System.Diagnostics.Activity.Current?.RootId
                };

                if (this.includeExceptionDetailsInResponse)
                {
                    currentException.InnerException = $"{innerException.Message} {innerException.StackTrace}";
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize<ExceptionResponse>(currentException)).ConfigureAwait(false);
            }
        }
    }
}
