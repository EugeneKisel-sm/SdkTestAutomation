using System.Text.Json.Serialization;

namespace SdkTestAutomation.Sdk.Implementations.Go.Models.Responses;

public class GoResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    public string Error { get; set; } = string.Empty;
    
    public string Message { get; set; } = string.Empty;
} 