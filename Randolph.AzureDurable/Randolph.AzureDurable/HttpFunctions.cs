using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Randolph.AzureDurable;

public static class HttpFunctions
{
    /// <summary>
    /// This function kicks-off the process.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="starter"></param>
    /// <param name="log"></param>
    /// <returns></returns>
    [FunctionName(nameof(ProcessVideoStarter))]
    public static async Task<IActionResult> ProcessVideoStarter(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
        HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        const string VideoKey = "video";
        
        var queryStringParams = req.GetQueryParameterDictionary();

        if (!queryStringParams.ContainsKey(VideoKey))
        {
            return new BadRequestObjectResult($"Expected a query string parameter: {VideoKey}");
        }

        string videoUrl = queryStringParams[VideoKey];
        
        // Function input comes from the request content.
        string instanceId = await starter.StartNewAsync(nameof(OrchestratorFunctions.ProcessVideoOrchestrator), null, videoUrl);

        log.LogInformation("Started orchestration with ID = '{InstanceId}'", instanceId);

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}