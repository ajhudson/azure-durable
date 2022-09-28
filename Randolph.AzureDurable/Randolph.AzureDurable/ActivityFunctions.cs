using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Randolph.AzureDurable;

public static class ActivityFunctions
{
    [FunctionName(nameof(TranscodeVideo))]
    public static async Task<string> TranscodeVideo([ActivityTrigger] string inputVideo, ILogger log)
    {
        const string fileSuffix = "transcoded.mp4";
        
        log.LogInformation("Transcoding {InputVideo}", inputVideo);
        
        // simulate doing the activity (replace with FFMPEG)
        await Task.Delay(5000);

        return $"{Path.GetFileNameWithoutExtension(inputVideo)}{fileSuffix}";
    }

    [FunctionName(nameof(ExtractThumbnail))]
    public static async Task<string> ExtractThumbnail([ActivityTrigger] string inputVideo, ILogger log)
    {
        const string fileSuffix = "thumbnail.png";
        
        log.LogInformation("Extracting thumbnail for {InputVideo}", inputVideo);
        
        // simulate doing the activity (replace with FFMPEG)
        await Task.Delay(5000);

        return $"{Path.GetFileNameWithoutExtension(inputVideo)}{fileSuffix}";
    }

    [FunctionName(nameof(PrependIntro))]
    public static async Task<string> PrependIntro([ActivityTrigger] string inputVideo, ILogger log)
    {
        const string fileSuffix = "withintro.mp4";
        
        // We are getting the intro location from environment variables rather than passing it in because it otherwise would have violated the orchestration function constraints
        // Alternatively we could have got this value before we called the orchestrator and passed it into the orchestrator itself which in turn would be passed to the activity function
        var introLocation = Environment.GetEnvironmentVariable("IntroLocation");
        log.LogInformation("Prepending Intro {IntroLocation} to {InputVideo}", introLocation, inputVideo);
        
        // simulate doing the activity (replace with FFMPEG)
        await Task.Delay(5000);
        
        return $"{Path.GetFileNameWithoutExtension(inputVideo)}{fileSuffix}";
    }
}
