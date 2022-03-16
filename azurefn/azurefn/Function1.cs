using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {




            var logger = executionContext.GetLogger("Function1");
            logger.LogInformation("C# HTTP trigger function processed a request. log");
            logger.LogWarning($"## The binding data keys are {string.Join(", ", executionContext.BindingContext.BindingData.Keys.ToList())}");
            logger.LogWarning($"## executionContext.FunctionDefinition.EntryPoint : {executionContext.FunctionDefinition.EntryPoint}");
            logger.LogWarning($"## executionContext.FunctionDefinition.Id : {executionContext.FunctionDefinition.Id}");
            logger.LogWarning($"##  executionContext.FunctionDefinition.InputBindings keys are {string.Join(", ", executionContext.FunctionDefinition.InputBindings.Keys.ToList())}");
            logger.LogWarning($"##  executionContext.FunctionDefinition.OutputBindings keys are {string.Join(", ", executionContext.FunctionDefinition.OutputBindings.Keys.ToList())}");
            logger.LogWarning($"## The parameter names are {string.Join(", ", executionContext.FunctionDefinition.Parameters.Select(x => x.Name + (x.Type.ToString())))}");
            logger.LogWarning($"## executionContext.FunctionId: {executionContext.FunctionId}");
            logger.LogWarning($"## executionContext.InvocationId: {executionContext.InvocationId}");
            logger.LogWarning($"## executionContext.RetryContext.MaxRetryCount: {GetMaxRetryCount(executionContext)}");
            logger.LogWarning($"## executionContext.RetryContext.RetryCount: {GetRetryCount(executionContext)}");
            logger.LogWarning($"## executionContext.TraceContext.TraceParent: {executionContext.TraceContext.TraceParent}");
            logger.LogWarning($"## executionContext.TraceContext.TraceState: {executionContext.TraceContext.TraceState}");

            //sending traceparent value as 00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-00 produced an empty string for local
            //sending tracestate value as congo=congosSecondPosition,rojo=rojosFirstPosition produced an empty string for local

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

        private static string GetMaxRetryCount(FunctionContext executionContext)
        {
            try
            {
                return executionContext.RetryContext.MaxRetryCount.ToString();
            }
            catch
            {
                return "Threw Exception";
            }
        }


        private static string GetRetryCount(FunctionContext executionContext)
        {
            try
            {
                return executionContext.RetryContext.RetryCount.ToString();
            }
            catch
            {
                return "Threw Exception";
            }
        }
    }
}
