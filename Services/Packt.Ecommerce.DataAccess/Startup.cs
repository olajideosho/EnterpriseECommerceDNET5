using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Packt.Ecommerce.Common.Middlewares;
using Packt.Ecommerce.Common.Options;
using Packt.Ecommerce.DataAccess.Extensions;
using Packt.Ecommerce.DataStore;

namespace Packt.Ecommerce.DataAccess
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
            services.Configure<DatabaseSettingsOptions>(this.Configuration.GetSection("CosmosDB"));
            string accountEndPoint = this.Configuration.GetValue<string>("CosmosDB:AccountEndPoint");
            string authKey = this.Configuration.GetValue<string>("CosmosDB:AuthKey");
            CosmosClientOptions options = new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions { IgnoreNullValues = true }
            };

            services.AddSingleton(s => new CosmosClient(accountEndPoint, authKey, options));
            services.AddRepositories();

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Packt.Ecommerce.DataAccess", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Packt.Ecommerce.DataAccess v1"));
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
    }
}
