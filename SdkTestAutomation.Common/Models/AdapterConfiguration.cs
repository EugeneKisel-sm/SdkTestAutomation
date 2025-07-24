using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

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
    
    [JsonProperty("javaHome")]
    public string JavaHome { get; set; }
    
    [JsonProperty("javaClassPath")]
    public string JavaClassPath { get; set; }
    
    [JsonProperty("enableLogging")]
    public bool EnableLogging { get; set; } = true;
    
    [JsonProperty("logLevel")]
    public string LogLevel { get; set; } = "Info";
} 