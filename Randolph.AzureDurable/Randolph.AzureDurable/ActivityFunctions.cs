using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Randolph.AzureDurable;

public static class ActivityFunctions
{
    private const int DelayMilliSeconds = 2000;
    
    [FunctionName(nameof(TranscodeVideo))]
    public static async Task<string> TranscodeVideo([ActivityTrigger] string inputVideo, ILogger log)
    {
        const string FileSuffix = "transcoded.mp4";
        
        log.LogInformation("Transcoding {InputVideo}", inputVideo);
        
        // simulate doing the activity (replace with FFMPEG)
        await Task.Delay(DelayMilliSeconds);

        return $"{Path.GetFileNameWithoutExtension(inputVideo)}{FileSuffix}";
    }

    [FunctionName(nameof(ExtractThumbnail))]
    public static async Task<string> ExtractThumbnail([ActivityTrigger] string inputVideo, ILogger log)
    {
        const string FileSuffix = "thumbnail.png";
        
        log.LogInformation("Extracting thumbnail for {InputVideo}", inputVideo);

        // simulate doing the activity (replace with FFMPEG)
        await Task.Delay(DelayMilliSeconds);

        // TODO this is a deliberate error to demo exception handling
        if (inputVideo.Contains("error"))
        {
            throw new InvalidCastException("Cannot get thumbnail");
        }

        return $"{Path.GetFileNameWithoutExtension(inputVideo)}{FileSuffix}";
    }

    [FunctionName(nameof(PrependIntro))]
    public static async Task<string> PrependIntro([ActivityTrigger] string inputVideo, ILogger log)
    {
        const string FileSuffix = "withintro.mp4";
        
        // We are getting the intro location from environment variables rather than passing it in because it otherwise would have violated the orchestration function constraints
        // Alternatively we could have got this value before we called the orchestrator and passed it into the orchestrator itself which in turn would be passed to the activity function
        var introLocation = Environment.GetEnvironmentVariable("IntroLocation");
        log.LogInformation("Prepending Intro {IntroLocation} to {InputVideo}", introLocation, inputVideo);
        
        // simulate doing the activity (replace with FFMPEG)
        await Task.Delay(DelayMilliSeconds);
        
        return $"{Path.GetFileNameWithoutExtension(inputVideo)}{FileSuffix}";
    }

    [FunctionName(nameof(CleanUp))]
    public static async Task<string> CleanUp([ActivityTrigger] string?[] filesToCleanUp, ILogger log)
    {
        var files = filesToCleanUp.Where(f => f != null);

        foreach (var currentFile in files)
        {
            log.LogInformation("Deleting file {FileName}", currentFile);
            await Task.Delay(DelayMilliSeconds);
        }

        return "Clean up successfully";
    }
}