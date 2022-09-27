using System.Collections.Generic;
using System.Threading.Tasks;
using Dynamitey;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Randolph.AzureDurable.Models;

namespace Randolph.AzureDurable;

public static class OrchestratorFunctions
{
    /// <summary>
    /// The function name attribute must match the name of the function itself
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [FunctionName(nameof(ProcessVideoOrchestrator))]
    public static async Task<ProcessVideoResult> ProcessVideoOrchestrator([OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        // We can get the input of any serializable type
        var videoLocation = context.GetInput<string>();
        
        // Pass onto the next function (this could be long-running). The function will sleep once a message has been queued to pass this part of the workflow on.
        // This also means no charge is incurred as the function is not being used
        var transcodedLocation = await context.CallActivityAsync<string>(nameof(ActivityFunctions.TranscodeVideo), videoLocation);

        var thumbnailLocation = await context.CallActivityAsync<string>(nameof(ActivityFunctions.ExtractThumbnail), transcodedLocation);

        var withIntroLocation = await context.CallActivityAsync<string>(nameof(ActivityFunctions.PrependIntro), thumbnailLocation);

        return new ProcessVideoResult(transcodedLocation, thumbnailLocation, withIntroLocation);
    }
}