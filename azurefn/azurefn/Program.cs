using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading.Tasks;

namespace azurefn
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services => {
                    TelemetryConfiguration telemetryConfig = TelemetryConfiguration.CreateDefault();
                    telemetryConfig.InstrumentationKey = GetEnvironmentVariable("InstrumentationKey");
                    TelemetryClient telemetryClient = new TelemetryClient(telemetryConfig);
                    var logger = new LoggerConfiguration()
                        .Enrich.WithProperty("applicationName", "FirstAzureFn")
                        .Enrich.WithProperty("Location", "USEast")
                   .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                   .WriteTo.Console()
                   .WriteTo.ApplicationInsights(telemetryClient, TelemetryConverter.Traces)
                   .CreateLogger();
                    services.AddLogging(lb => lb.AddSerilog(logger));
                    services.AddSingleton<TelemetryClient>(telemetryClient);
                })
                .Build();

            host.Run();
        }
        private static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}