using Newtonsoft.Json;
using SdkTestAutomation.Utils;

namespace SdkTestAutomation.Sdk.Models;

public class AdapterConfiguration
{
    [JsonProperty("serverUrl")] 
    public string ServerUrl { get; set; } = TestConfig.ApiUrl;
    
    [JsonProperty("pythonHome")]
    public string PythonHome { get; set; }
    
    [JsonProperty("pythonPath")]
    public string PythonPath { get; set; }
} 