using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Common.Interfaces;

/// <summary>
/// Interface for event resource operations across different SDKs
/// </summary>
public interface IEventResourceAdapter : ISdkAdapter
{
    /// <summary>
    /// Add a new event handler
    /// </summary>
    Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request);
    
    /// <summary>
    /// Get all event handlers
    /// </summary>
    Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request);
    
    /// <summary>
    /// Get event handlers by event name
    /// </summary>
    Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request);
    
    /// <summary>
    /// Update an existing event handler
    /// </summary>
    Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request);
    
    /// <summary>
    /// Delete an event handler
    /// </summary>
    Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request);
} 