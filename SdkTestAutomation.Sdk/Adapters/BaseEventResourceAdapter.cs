using SdkTestAutomation.Sdk.Interfaces;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;

namespace SdkTestAutomation.Sdk.Adapters;

public abstract class BaseEventResourceAdapter : BaseConductorAdapter, IEventResourceAdapter
{
    protected readonly EventInfoMapper EventMapper = new();
    
    public abstract SdkResponse<GetEventResponse> AddEvent(AddEventRequest request);
    public abstract SdkResponse<GetEventResponse> GetEvent(GetEventRequest request);
    public abstract SdkResponse<GetEventResponse> GetEventByName(GetEventByNameRequest request);
    public abstract SdkResponse<GetEventResponse> UpdateEvent(UpdateEventRequest request);
    public abstract SdkResponse<GetEventResponse> DeleteEvent(DeleteEventRequest request);
    
    protected static GetEventResponse CreateResponseFromRequest(dynamic request) => new()
    {
        Name = request.Name,
        Event = request.Event,
        Active = request.Active,
        Actions = request.Actions,
        Condition = request.Condition,
        EvaluatorType = request.EvaluatorType
    };
} 