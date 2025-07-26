using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Adapters;

namespace SdkTestAutomation.Java;

public class ConductorJavaEventResourceAdapter : BaseEventResourceAdapter
{
    private JavaConductorClient JavaClient => (JavaConductorClient)Client;
    public override string SdkType => "java";
    
    protected override ConductorClient CreateClient(string serverUrl) => new JavaConductorClient(serverUrl);
    
    public override SdkResponse<GetEventResponse> AddEvent(AddEventRequest request)
    {
        try
        {
            var eventHandler = CreateEventHandler(request);
            JavaClient.EventClient.registerEventHandler(eventHandler);
            return SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> GetEvent(GetEventRequest request)
    {
        try
        {
            var events = JavaClient.EventClient.getEventHandlers("", false);
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventMapper.MapFromJava(firstEvent));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> GetEventByName(GetEventByNameRequest request)
    {
        try
        {
            var events = JavaClient.EventClient.getEventHandlers(request.Event, request.ActiveOnly ?? false);
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventMapper.MapFromJava(firstEvent));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> UpdateEvent(UpdateEventRequest request)
    {
        try
        {
            var eventHandler = CreateEventHandler(request);
            JavaClient.EventClient.updateEventHandler(eventHandler);
            return SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> DeleteEvent(DeleteEventRequest request)
    {
        try
        {
            JavaClient.EventClient.unregisterEventHandler(request.Name);
            return SdkResponse<GetEventResponse>.CreateSuccess(new GetEventResponse());
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    private dynamic CreateEventHandler(dynamic request)
    {
        var eventHandler = JavaClient.EventClient.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler");
        eventHandler.setName(request.Name);
        eventHandler.setEvent(request.Event);
        eventHandler.setActive(request.Active);
        return eventHandler;
    }
    
    protected override string GetSdkVersion() => SdkVersionHelper.GetTypeVersion(
        "com.netflix.conductor.client.http.ConductorClient", "conductor-client");
} 