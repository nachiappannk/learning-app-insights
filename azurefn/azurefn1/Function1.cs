using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azurefn1
{
    public class Function1
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly TelemetryClient telemetryClient;

        public Function1(Test client, IHttpClientFactory clientFactory, TelemetryClient telemetryClient)
        {
            this.clientFactory = clientFactory;
            this.telemetryClient = telemetryClient;
        }

        public static string RandomString(int length)
        {

            Random random = new Random();
            const string chars = "abcdef0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [Function("Function1112")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Function1");
            logger.LogInformation("C# HTTP trigger function processed a request.");
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

            logger.LogWarning($"## telemetryClient.Context.Operation.Id: {telemetryClient.Context.Operation.Id}");
            logger.LogWarning($"## telemetryClient.Context.Operation.ParentId: {telemetryClient.Context.Operation.ParentId}");
            logger.LogWarning($"## telemetryClient.Context.Operation.Name: {telemetryClient.Context.Operation.Name}");


            DependencyTelemetry telemetry = new DependencyTelemetry();
            telemetry.Context.Operation.Id = $"nachi-{RandomString(16)}";
            telemetry.Context.Operation.ParentId = $"nachi-parent-{RandomString(16)}";
            telemetry.Context.Operation.Name = $"nachi-name-{RandomString(16)}";
            telemetry.Id = $"nachi-telemetyid-{RandomString(16)}";
            telemetry.Properties.Add("one", "tow");
            telemetry.Data = "calling http something";
            telemetry.ResultCode = "sucess";
            telemetry.Success = true;
            telemetry.Type = "HTTP";
            telemetry.Duration = new TimeSpan(0, 0, 0, 0, 200);
            telemetry.Name = "name-nachi";
            telemetry.Target = "dflkajs-target";
            telemetry.Timestamp = DateTime.UtcNow;



            telemetryClient.TrackDependency(telemetry);
 

            try
            {
                throw new Exception("some random exception");

            } catch (Exception e)
            {
                logger.LogError(e, "This is a random failure");
            }

            var client = clientFactory.CreateClient();

            var traceParent = executionContext.TraceContext.TraceParent;
            if (!string.IsNullOrEmpty(traceParent))
            {
                var ss = traceParent.Split('-');
                ss[2] = RandomString(16);
                var newTraceParent = string.Join('-', ss);
                client.DefaultRequestHeaders.Add("traceparent", newTraceParent);
            }
            
            var response1  = await client.GetAsync("https://azurefnnachi1.azurewebsites.net/api/Function11125");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Welcome to Azure Functions!");

            return response;
        }


        [Function("Function11125")]
        public HttpResponseData Run1([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Function1");
            logger.LogInformation("Another call");
            try
            {
                throw new Exception("some random exception");

            }
            catch (Exception e)
            {
                logger.LogError(e, "This is a random failure");
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("Welcome to Azure Functions!");
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
