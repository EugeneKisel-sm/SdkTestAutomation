using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SdkTestAutomation.Sdk.Core.Models;

public class SdkResponse<T>
{
    public bool Success { get; set; }
    public string Content { get; set; } = string.Empty;
    public T Data { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
} 

public class SdkResponse : SdkResponse<object>
{
    public static SdkResponse CreateSuccess(object data = null)
    {
        var response = new SdkResponse
        {
            StatusCode = HttpStatusCode.OK,
            Success = true,
            Data = data
        };

        if (data != null)
        {
            response.Content = JsonConvert.SerializeObject(data,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
        }

        return response;
    }

    public static SdkResponse CreateError(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new SdkResponse
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Success = false,
            ErrorMessage = message
        };
    }
}