using Newtonsoft.Json;

namespace SdkTestAutomation.Sdk.Models;

/// <summary>
/// Simplified configuration for SDK adapters
/// </summary>
public class AdapterConfiguration
{
    [JsonProperty("serverUrl")]
    public string ServerUrl { get; set; } = "http://localhost:8080/api";
    
    [JsonProperty("pythonHome")]
    public string PythonHome { get; set; }
    
    [JsonProperty("pythonPath")]
    public string PythonPath { get; set; }
} 