using Newtonsoft.Json;

namespace SdkTestAutomation.Sdk.Models;

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
    public static SdkResponse CreateSuccess(object data = null)
    {
        var response = new SdkResponse
        {
            StatusCode = 200,
            Success = true,
            Data = data
        };
        
        if (data != null)
        {
            response.Content = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions 
            { 
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase 
            });
        }
        
        return response;
    }
    
    public static SdkResponse CreateError(int statusCode, string errorMessage)
    {
        return new SdkResponse
        {
            StatusCode = statusCode,
            Success = false,
            ErrorMessage = errorMessage
        };
    }
} 