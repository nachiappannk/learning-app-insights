using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ApplicationInsights;
using System;
using Microsoft.ApplicationInsights.DataContracts;

namespace azurefn1
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureLogging((context, builder) => {
                    builder.AddApplicationInsights("2811a959-1208-4f03-8ef3-223560281d0e");
                })
                .ConfigureServices( serviceProvider =>
                {
                    serviceProvider
                        .AddHttpClient()
                        .AddLogging()
                        .AddSingleton<Test>()
                        .AddSingleton(s => GetTelemetryClient());//check scoped
                    ;
                })
                .Build();

            host.Run();
        }

        private static TelemetryClient GetTelemetryClient()
        {
            TelemetryConfiguration telemetryConfig = TelemetryConfiguration.CreateDefault();
            telemetryConfig.TelemetryInitializers.Add(new CloudRoleNameTelemetryInitializer());
            telemetryConfig.InstrumentationKey = GetEnvironmentVariable("InstrumentationKey");
            TelemetryClient telemetryClient = new TelemetryClient(telemetryConfig);
            return telemetryClient;
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }

    public class Test 
    { 
    }


    public class CloudRoleNameTelemetryInitializer : ITelemetryInitializer
    {

        public CloudRoleNameTelemetryInitializer()
        {
            
        }

        public void Initialize(ITelemetry telemetry)
        {
            // set custom role name here
            telemetry.Context.Cloud.RoleName = "newnachiazurefn";
            var requestTelemetry = telemetry as DependencyTelemetry;
            if (requestTelemetry == null) return;
            requestTelemetry.Properties.Add("LoggedInUserName", "DummyUser");

        }
    }
}