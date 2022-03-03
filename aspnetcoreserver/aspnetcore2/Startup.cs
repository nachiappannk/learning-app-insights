using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspnetcore2
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
            services.AddApplicationInsightsTelemetry();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "aspnetcore2", Version = "v1" });
            });
            services.AddHttpClient();
            //services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, options) =>
            //{
            //    // disable all dependency collection
            //    module.DisableDiagnosticSourceInstrumentation = false;

            //    // disable command text
            //    module.EnableSqlCommandTextInstrumentation = false;
            //});

            services.AddApplicationInsightsTelemetryProcessor<AzureDependencyFilterTelemetryProcessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "aspnetcore2 v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class AzureDependencyFilterTelemetryProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _inner;

        public AzureDependencyFilterTelemetryProcessor(ITelemetryProcessor inner)
        {
            _inner = inner;
        }  

        public void Process(ITelemetry item)
        {
            if (item is Microsoft.ApplicationInsights.DataContracts.DependencyTelemetry dependency1) 
            {
                if (dependency1.Target == "aspnetcoreserver1.azurewebsites.net") 
                {
                    dependency1.Target = "aspnetcoreserver1";
                }
            }
            _inner.Process(item);
        }
    }

}
