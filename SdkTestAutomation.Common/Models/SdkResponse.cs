using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Standardized response structure for all SDK operations
/// </summary>
public class SdkResponse<T>
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }
    
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("data")]
    public T? Data { get; set; }
    
    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;
    
    [JsonProperty("requestId")]
    public string RequestId { get; set; } = string.Empty;
    
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
} 