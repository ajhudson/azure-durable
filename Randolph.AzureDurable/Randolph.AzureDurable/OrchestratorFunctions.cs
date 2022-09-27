using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Randolph.AzureDurable;

public static class OrchestratorFunctions
{
    [FunctionName("Starter")]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var outputs = new List<string>();

        // Replace "hello" with the name of your Durable Activity Function.
        outputs.Add(await context.CallActivityAsync<string>("Starter_Hello", "Tokyo"));
        outputs.Add(await context.CallActivityAsync<string>("Starter_Hello", "Seattle"));
        outputs.Add(await context.CallActivityAsync<string>("Starter_Hello", "London"));

        // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
        
    }
}