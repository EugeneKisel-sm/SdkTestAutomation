using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

public class SdkCommand
{
    [JsonProperty("operation")]
    public string Operation { get; set; } = string.Empty;
    
    [JsonProperty("parameters")]
    public Dictionary<string, object> Parameters { get; set; } = new();
    
    [JsonProperty("resource")]
    public string Resource { get; set; } = string.Empty;
} 