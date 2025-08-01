using System.Text.Json.Serialization;

namespace SdkTestAutomation.Sdk.Implementations.Java;

public class JavaResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("data")]
    public object Data { get; set; }
    
    [JsonPropertyName("errorMessage")]
    public string Error { get; set; } = string.Empty;
    
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
} 