using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Event action model
/// </summary>
public class EventAction
{
    [JsonProperty("action")]
    public string Action { get; set; } = string.Empty;
    
    [JsonProperty("startWorkflow")]
    public StartWorkflow? StartWorkflow { get; set; }
} 