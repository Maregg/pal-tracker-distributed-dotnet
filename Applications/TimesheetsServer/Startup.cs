using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Timesheets;

using Swashbuckle.AspNetCore.Swagger;
using Pivotal.Discovery.Client;
using Steeltoe.Common.Discovery;
using Steeltoe.CircuitBreaker.Hystrix;

namespace TimesheetsServer
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
            // Add framework services.
            services.AddMvc();

            services.AddDbContext<TimeEntryContext>(options => options.UseMySql(Configuration));
            services.AddScoped<ITimeEntryDataGateway, TimeEntryDataGateway>();
            
            services.AddSingleton<IProjectClient>(sp =>
            {
                var handler = new DiscoveryHttpClientHandler(sp.GetService<IDiscoveryClient>());
                var httpClient = new HttpClient(handler, false)
                {
                    BaseAddress = new Uri(Configuration.GetValue<string>("REGISTRATION_SERVER_ENDPOINT"))
                };

                var logger = sp.GetService<ILogger<ProjectClient>>();
                return new ProjectClient(httpClient, logger);
            });
            
            services.AddHystrixMetricsStream(Configuration);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Timesheet Server", Version = "v1" });
                //c.IncludeXmlComments($@"{System.AppDomain.CurrentDomain.BaseDirectory}/edu.gateway.api.xml");
                c.DescribeAllEnumsAsStrings();
            });

            services.AddDiscoveryClient(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TimesheetServer");
            });

            app.UseMvc();

            app.UseDiscoveryClient();
            
            app.UseHystrixMetricsStream();
            app.UseHystrixRequestContext();
        }
    }
}