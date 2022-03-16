using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace azurefn1
{
    public static class Function1
    {
        [Function("Function1112")]
        public static HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
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

            try
            {
                throw new Exception("some random exception");

            } catch (Exception e)
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
