using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Start workflow model
/// </summary>
public class StartWorkflow
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("version")]
    public int Version { get; set; } = 1;
    
    [JsonProperty("input")]
    public Dictionary<string, object> Input { get; set; } = new();
} 