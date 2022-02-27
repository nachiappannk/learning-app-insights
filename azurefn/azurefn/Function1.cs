using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace azurefn
{
    public class Function1
    {
        private readonly TelemetryClient telemetryClient;

        public Function1(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Function1");
            logger.LogInformation("C# HTTP trigger function processed a request. log");

            HttpClient client = new HttpClient();
            var response1 = await client.GetAsync("https://google.com/");
            DependencyTelemetry dependencyTelemetry = new DependencyTelemetry();
            telemetryClient.TrackDependency("NachiHttpClient", "NachiGoogle", new System.DateTimeOffset(System.DateTime.Now), new System.TimeSpan(0, 0, 0, 0, 10), true);
            //todo modify this

            Random rnd = new Random();
            var randNumber = rnd.Next(0, 100);
            if (randNumber < 12) 
            {
                throw new ApplicationException("some application exception");
            }
            if (randNumber < 20)
            {
                throw new Exception("some exception");
            }


            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString($"Welcome to Azure Functions! {response1.StatusCode}");

            return response;
        }
    }
}
