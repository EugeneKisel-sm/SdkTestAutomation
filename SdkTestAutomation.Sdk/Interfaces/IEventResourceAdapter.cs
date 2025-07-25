using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;

namespace SdkTestAutomation.Sdk.Interfaces;

public interface IEventResourceAdapter : ISdkAdapter
{
    SdkResponse<GetEventResponse> AddEvent(AddEventRequest request);
    
    SdkResponse<GetEventResponse> GetEvent(GetEventRequest request);
    
    SdkResponse<GetEventResponse> GetEventByName(GetEventByNameRequest request);
    
    SdkResponse<GetEventResponse> UpdateEvent(UpdateEventRequest request);
    
    SdkResponse<GetEventResponse> DeleteEvent(DeleteEventRequest request);
} 