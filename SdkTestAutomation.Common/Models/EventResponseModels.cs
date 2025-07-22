using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Response for event operations
/// </summary>
public class GetEventResponse
{
    [JsonProperty("events")]
    public List<EventInfo> Events { get; set; } = new();
    
    [JsonProperty("count")]
    public int Count => Events.Count;
} 