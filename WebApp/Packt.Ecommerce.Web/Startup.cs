using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.SnapshotCollector;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Packt.Ecommerce.Common.HealthCheck;
using Packt.Ecommerce.Common.Options;
using Packt.Ecommerce.Web.Contracts;
using Packt.Ecommerce.Web.Services;
using Polly;
using Polly.Extensions.Http;

namespace Packt.Ecommerce.Web
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
            services.Configure<ApplicationSettings>(this.Configuration.GetSection("ApplicationSettings"));
            services.AddControllersWithViews();
            services.AddHttpClient<IECommerceService, ECommerceService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                .AddPolicyHandler(RetryPolicy())
                .AddPolicyHandler(CircuitBreakerPolicy());
            services.AddScoped<IECommerceService, ECommerceService>();

            //services.AddApplicationInsightsTelemetry(this.Configuration["ApplicationInsights:InstrumentationKey"]);
            //services.AddSnapshotCollector((configuration) => this.Configuration.Bind(nameof(SnapshotCollectorConfiguration), configuration));


            services.AddHealthChecks()
                .AddUrlGroup(new Uri(this.Configuration.GetValue<string>("ApplicationSettings:ProductsApiEndpoint")), name: "Product Service")
                .AddUrlGroup(new Uri(this.Configuration.GetValue<string>("ApplicationSettings:OrdersApiEndpoint")), name: "Order Service");
                //.AddProcessMonitorHealthCheck("notepad", name: "Notepad monitor");

            //services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            //    .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAdB2C"));

            services.AddRazorPages().AddMicrosoftIdentityUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStatusCodePagesWithReExecute("/Products/Error/{0}");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Products/Error/500");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = async (context, report) =>
                    {
                        context.Response.ContentType = "application/json";
                        var response = new
                        {
                            Status = report.Status.ToString(),
                            HealthChecks = report.Entries.Select(x => new
                            {
                                Component = x.Key,
                                Status = x.Value.Status.ToString(),
                                Description = x.Value.Description
                            }),
                            HealthCheckDuration = report.TotalDuration
                        };

                        await context.Response.WriteAsync(JsonSerializer.Serialize(response)).ConfigureAwait(false);
                    }
                });
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
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
        }

    }
}

















//Global Authorization
//services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//});

//Policy-Based Role Authorization
//services.AddAuthorization(options =>
//{
//    options.AddPolicy("OrderAccessPolicy", options =>
//    {
//        options.RequireRole("User", "Support");
//    });
//});