namespace Randolph.AzureDurable.Models;

public record ProcessVideoResult(string? Transcoded, string? Thumbnail, string? WithIntroLocation, bool Success, string? ErrorMessage);