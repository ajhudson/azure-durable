using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Randolph.AzureDurable.Models;

namespace Randolph.AzureDurable;

public static class OrchestratorFunctions
{
    /// <summary>
    /// The function name attribute must match the name of the function itself
    /// </summary>
    /// <param name="context"></param>
    /// <param name="log"></param>
    /// <returns></returns>
    [FunctionName(nameof(ProcessVideoOrchestrator))]
    public static async Task<ProcessVideoResult> ProcessVideoOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context, ILogger log)
    {
        var safeLogger = context.CreateReplaySafeLogger(log);

        // We can get the input of any serializable type
        var videoLocation = context.GetInput<string>();
        safeLogger.LogInformation("Got the {VideoLocation}", videoLocation);

        // Pass onto the next function (this could be long-running). The function will sleep once a message has been queued to pass this part of the workflow on.
        // This also means no charge is incurred as the function is not being used
        void LogAboutToInvoke(string functionName) => safeLogger.LogInformation("About to invoke {FunctionName}", functionName);

        LogAboutToInvoke(nameof(ActivityFunctions.TranscodeVideo));
        var transcodedLocation = await context.CallActivityAsync<string>(nameof(ActivityFunctions.TranscodeVideo), videoLocation);

        LogAboutToInvoke(nameof(ActivityFunctions.ExtractThumbnail));
        var thumbnailLocation = await context.CallActivityAsync<string>(nameof(ActivityFunctions.ExtractThumbnail), transcodedLocation);

        LogAboutToInvoke(nameof(ActivityFunctions.PrependIntro));
        var withIntroLocation = await context.CallActivityAsync<string>(nameof(ActivityFunctions.PrependIntro), thumbnailLocation);

        return new ProcessVideoResult(transcodedLocation, thumbnailLocation, withIntroLocation);
    }
}
