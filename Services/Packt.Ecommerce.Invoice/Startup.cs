using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Packt.Ecommerce.Caching;
using Packt.Ecommerce.Caching.Interfaces;
using Packt.Ecommerce.Common.Middlewares;
using Packt.Ecommerce.Common.Options;
using Packt.Ecommerce.Invoice.Contracts;
using Packt.Ecommerce.Invoice.Services;
using Polly;
using Polly.Extensions.Http;

namespace Packt.Ecommerce.Invoice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.Configure<ApplicationSettings>(this.Configuration.GetSection("ApplicationSettings"));

            services.AddHttpClient<IInvoiceService, InvoiceService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(RetryPolicy())
                .AddPolicyHandler(CircuitBreakerPolicy());

            services.AddScoped<IInvoiceService, InvoiceService>();

            services.AddAutoMapper(typeof(AutoMapperProfile));

            services.AddSingleton<IEntitySerializer, EntitySerializer>();
            services.AddSingleton<IDistributedCacheService, DistributedCacheService>();

            if (this.Configuration.GetValue<bool>("ApplicationSettings:UseRedis"))
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = this.Configuration.GetConnectionString("Redis");
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            string appinsightsInstrumentationKey = this.Configuration.GetValue<string>("ApplicationSettings:InstrumentationKey");

            if (!string.IsNullOrWhiteSpace(appinsightsInstrumentationKey))
            {
                services.AddLogging(logging =>
                {
                    logging.AddApplicationInsights(appinsightsInstrumentationKey);
                });
                services.AddApplicationInsightsTelemetry(appinsightsInstrumentationKey);
            }

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Packt.Ecommerce.Invoice", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Packt.Ecommerce.Invoice v1"));
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static IAsyncPolicy<HttpResponseMessage> RetryPolicy()
        {
            Random random = new Random();
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(
                5,
                retry => TimeSpan.FromSeconds(Math.Pow(2, retry))
                                  + TimeSpan.FromMilliseconds(random.Next(0, 100)));
            return retryPolicy;
        }

        private static IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy()
        {
            return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }
    }
}
