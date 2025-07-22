using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Request for updating an event handler
/// </summary>
public class UpdateEventRequest : SdkRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;
    
    [JsonProperty("active")]
    public bool Active { get; set; } = true;
    
    [JsonProperty("actions")]
    public List<EventAction> Actions { get; set; } = new();
} 