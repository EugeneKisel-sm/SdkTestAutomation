using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Python.Helpers;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Adapters;
using Python.Runtime;

namespace SdkTestAutomation.Python;

public class ConductorPythonEventResourceAdapter : BaseEventResourceAdapter
{
    private readonly PythonEventHandlerBuilder _eventHandlerBuilder = new();
    private PythonConductorClient PythonClient => (PythonConductorClient)Client;
    public override string SdkType => "python";
    
    protected override ConductorClient CreateClient(string serverUrl) => new PythonConductorClient(serverUrl);
    
    public override SdkResponse<GetEventResponse> AddEvent(AddEventRequest request)
    {
        try
        {
            PythonClient.ExecuteWithGIL(() => 
            {
                var eventHandler = _eventHandlerBuilder.CreateEventHandler(request);
                PythonClient.EventClient.register_event_handler(eventHandler);
            });
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
            var events = PythonClient.ExecuteWithGIL(() => PythonClient.EventClient.get_event_handlers("", false));
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventMapper.MapFromPython(firstEvent));
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
            var events = PythonClient.ExecuteWithGIL(() => 
                PythonClient.EventClient.get_event_handlers(request.Event, request.ActiveOnly ?? false));
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventMapper.MapFromPython(firstEvent));
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
            PythonClient.ExecuteWithGIL(() => 
            {
                var eventHandler = _eventHandlerBuilder.CreateEventHandler(request);
                PythonClient.EventClient.update_event_handler(eventHandler);
            });
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
            PythonClient.ExecuteWithGIL(() => PythonClient.EventClient.unregister_event_handler(request.Name));
            return SdkResponse<GetEventResponse>.CreateSuccess(new GetEventResponse());
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    protected override string GetSdkVersion() => SdkVersionHelper.GetModuleVersion(() => 
        PythonClient.ExecuteWithGIL(() => Py.Import("conductor")));
} 