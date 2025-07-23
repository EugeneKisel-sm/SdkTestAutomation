using Newtonsoft.Json;

namespace SdkTestAutomation.Sdk.Models;

public class SdkCommand
{
    [JsonProperty("resource")]
    public string Resource { get; set; } = string.Empty;
    
    [JsonProperty("parameters")]
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    [JsonProperty("operation")]
    public string Operation { get; set; } = string.Empty;
} 