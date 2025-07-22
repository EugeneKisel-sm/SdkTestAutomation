using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Request for getting event handlers by name
/// </summary>
public class GetEventByNameRequest : SdkRequest
{
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;
    
    [JsonProperty("activeOnly")]
    public bool? ActiveOnly { get; set; }
} 