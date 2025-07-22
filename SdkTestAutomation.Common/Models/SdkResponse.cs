using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

public class SdkResponse<T>
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }
    
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("data")]
    public T Data { get; set; }
    
    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;
}

public class SdkResponse : SdkResponse<object>
{
} 