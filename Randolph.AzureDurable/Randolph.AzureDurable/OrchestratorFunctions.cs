using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dynamitey;
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
        // Get a safe logger
        var safeLogger = context.CreateReplaySafeLogger(log);
        void Log(string methodName) => safeLogger.LogInformation("About to call {MethodName}", methodName);
        
        // We can get the input of any serializable type
        var videoLocation = context.GetInput<string>();
        
        // Pass onto the next function (this could be long-running). The function will sleep once a message has been queued to pass this part of the workflow on.
        // This also means no charge is incurred as the function is not being used

        string? transcodedLocation = null;
        string? thumbnailLocation = null;
        string? withIntroLocation = null;

        try
        {
            Log(nameof(ActivityFunctions.TranscodeVideo));
            transcodedLocation = await context.CallActivityAsync<string?>(nameof(ActivityFunctions.TranscodeVideo), videoLocation);
            
            Log(nameof(ActivityFunctions.ExtractThumbnail));
            thumbnailLocation = await context.CallActivityAsync<string?>(nameof(ActivityFunctions.ExtractThumbnail), transcodedLocation);
            
            Log(nameof(ActivityFunctions.PrependIntro));
            withIntroLocation = await context.CallActivityAsync<string?>(nameof(ActivityFunctions.PrependIntro), thumbnailLocation);
        }
        catch (Exception e)
        {
            safeLogger.LogError("Caught an error from an activity: {Message}", e.Message);
            
            await context.CallActivityAsync<string>("CleanUp", new[] { transcodedLocation, thumbnailLocation, withIntroLocation });

            return new ProcessVideoResult(transcodedLocation, thumbnailLocation, withIntroLocation, false, e.Message);
        }

        return new ProcessVideoResult(transcodedLocation, thumbnailLocation, withIntroLocation, true, null);
    }
}