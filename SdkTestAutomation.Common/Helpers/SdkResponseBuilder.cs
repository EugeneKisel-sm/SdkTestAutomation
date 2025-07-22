using System.Text.Json;
using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Common.Helpers;

/// <summary>
/// Shared helper class for building SdkResponse objects across all SDKs
/// </summary>
public static class SdkResponseBuilder
{
    /// <summary>
    /// Create a successful response
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateSuccessResponse(
        GetEventResponse data, 
        string requestId)
    {
        var response = new SdkResponse<GetEventResponse>
        {
            Success = true,
            StatusCode = 200,
            RequestId = requestId,
            Timestamp = DateTime.UtcNow,
            Data = data
        };
        
        response.Content = JsonSerializer.Serialize(data);
        return response;
    }
    
    /// <summary>
    /// Create an error response
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateErrorResponse(
        string errorMessage, 
        string requestId, 
        int statusCode = 500)
    {
        return new SdkResponse<GetEventResponse>
        {
            Success = false,
            StatusCode = statusCode,
            ErrorMessage = errorMessage,
            RequestId = requestId,
            Timestamp = DateTime.UtcNow
        };
    }
    
    /// <summary>
    /// Create a response from request data (for add/update operations)
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateFromRequest(
        AddEventRequest request)
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
        
        return CreateSuccessResponse(data, request.RequestId);
    }
    
    /// <summary>
    /// Create a response from update request data
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateFromRequest(
        UpdateEventRequest request)
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
        
        return CreateSuccessResponse(data, request.RequestId);
    }
    
    /// <summary>
    /// Create an empty response
    /// </summary>
    public static SdkResponse<GetEventResponse> CreateEmptyResponse(string requestId)
    {
        var data = new GetEventResponse
        {
            Events = new List<EventInfo>()
        };
        
        return CreateSuccessResponse(data, requestId);
    }
} 