using System.Net;

namespace SdkTestAutomation.Sdk.Core.Models;

public class SdkResponse
{
    public bool Success { get; set; }
    public string Content { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
    
    public static SdkResponse CreateSuccess(string content = "") => new() { Success = true, Content = content };
    public static SdkResponse CreateError(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) => new() { Success = false, ErrorMessage = message, StatusCode = statusCode };
} 