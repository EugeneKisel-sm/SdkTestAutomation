using Newtonsoft.Json;

namespace SdkTestAutomation.Sdk.Models;

public class SdkResponse<T>
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("data")]
    public T Data { get; set; }
    
    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;
    
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; } = 200;
    
    public static SdkResponse<T> CreateSuccess(T data) => new() { Success = true, Data = data };
    public static SdkResponse<T> CreateError(string message, int statusCode = 500) => new() { Success = false, ErrorMessage = message, StatusCode = statusCode };
} 