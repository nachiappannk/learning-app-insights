using Microsoft.Azure.Functions.Worker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

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
                .ConfigureServices( x => {
                    x
                        .AddHttpClient()
                        .AddLogging()
                        .AddSingleton<Test>();
                })
                .Build();

            host.Run();
        }
    }

    public class Test 
    { 
    }
}