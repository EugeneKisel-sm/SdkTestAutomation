using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Event information model
/// </summary>
public class EventInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;
    
    [JsonProperty("active")]
    public bool Active { get; set; }
    
    [JsonProperty("actions")]
    public List<EventAction> Actions { get; set; } = new();
    
    [JsonProperty("condition")]
    public string Condition { get; set; }
    
    [JsonProperty("evaluatorType")]
    public string EvaluatorType { get; set; }
} 