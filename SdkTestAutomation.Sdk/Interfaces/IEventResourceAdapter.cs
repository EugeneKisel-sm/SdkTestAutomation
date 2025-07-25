using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;

namespace SdkTestAutomation.Sdk.Interfaces;

public interface IEventResourceAdapter : ISdkAdapter
{
    Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request);
    
    Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request);
    
    Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request);
    
    Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request);
    
    Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request);
} 