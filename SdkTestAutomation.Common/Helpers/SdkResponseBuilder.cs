using System.Text.Json;
using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Common.Helpers;

/// <summary>
/// Simplified helper for building SdkResponse objects
/// </summary>
public static class SdkResponseBuilder
{
    /// <summary>
    /// Create a successful response with data
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateSuccessResponse(GetEventResponse data)
    {
        var response = SdkResponse<GetEventResponse>.CreateSuccess(data);
        response.Content = JsonSerializer.Serialize(data);
        return response;
    }
    
    /// <summary>
    /// Create an error response
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateErrorResponse(string errorMessage, int statusCode = 500)
    {
        return SdkResponse<GetEventResponse>.CreateError(errorMessage, statusCode);
    }
    
    /// <summary>
    /// Create a response from request data (for add/update operations)
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateFromRequest(dynamic request)
    {
        var eventInfo = new EventInfo
        {
            Name = request.Name,
            Event = request.Event,
            Active = request.Active,
            Actions = request.Actions
        };
        
        var data = new GetEventResponse
        {
            Events = new List<EventInfo> { eventInfo }
        };
        
        return CreateSuccessResponse(data);
    }
    
    /// <summary>
    /// Create an empty response
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateEmptyResponse()
    {
        var data = new GetEventResponse
        {
            Events = new List<EventInfo>()
        };
        
        return CreateSuccessResponse(data);
    }
} 