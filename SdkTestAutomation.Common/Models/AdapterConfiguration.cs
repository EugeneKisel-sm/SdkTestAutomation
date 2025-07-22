using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Configuration for SDK adapters
/// </summary>
public class AdapterConfiguration
{
    [JsonProperty("serverUrl")]
    public string ServerUrl { get; set; } = "http://localhost:8080/api";
    
    [JsonProperty("timeout")]
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    
    [JsonProperty("maxRetries")]
    public int MaxRetries { get; set; } = 3;
    
    [JsonProperty("retryDelay")]
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    
    [JsonProperty("pythonHome")]
    public string? PythonHome { get; set; }
    
    [JsonProperty("pythonPath")]
    public string? PythonPath { get; set; }
    
    [JsonProperty("javaHome")]
    public string? JavaHome { get; set; }
    
    [JsonProperty("javaClassPath")]
    public string? JavaClassPath { get; set; }
    
    [JsonProperty("javaOptions")]
    public List<string> JavaOptions { get; set; } = new()
    {
        "-Xmx512m",
        "-Xms256m"
    };
    
    [JsonProperty("enableLogging")]
    public bool EnableLogging { get; set; } = true;
    
    [JsonProperty("logLevel")]
    public string LogLevel { get; set; } = "Info";
} 