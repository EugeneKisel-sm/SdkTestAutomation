using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Base class for all SDK requests
/// </summary>
public abstract class SdkRequest
{
    [JsonProperty("requestId")]
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
} 