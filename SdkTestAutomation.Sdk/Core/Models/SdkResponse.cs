namespace SdkTestAutomation.Sdk.Core.Models;

public class SdkResponse
{
    public bool Success { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;
    
    public static SdkResponse CreateSuccess(string content = "") => new() { Success = true, Content = content };
    public static SdkResponse CreateError(string message, int statusCode = 500) => new() { Success = false, ErrorMessage = message, StatusCode = statusCode };
} 