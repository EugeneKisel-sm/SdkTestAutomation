using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;
using Python.Runtime;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public class PythonEventAdapter : IEventAdapter
{
    private PythonClient _client;
    
    public string SdkType => "python";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new PythonClient();
            _client.Initialize(serverUrl);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public SdkResponse AddEvent(string name, string eventType, bool active = true)
    {
        try
        {
            using (Py.GIL())
            {
                var eventHandler = CreateEventHandler(name, eventType, active);
                _client.EventApi.register_event_handler(eventHandler);
            }
            return SdkResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse GetEvents()
    {
        try
        {
            using (Py.GIL())
            {
                var events = _client.EventApi.get_event_handlers("", false);
                return SdkResponse.CreateSuccess(Newtonsoft.Json.JsonConvert.SerializeObject(events));
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse GetEventByName(string eventName)
    {
        try
        {
            using (Py.GIL())
            {
                var events = _client.EventApi.get_event_handlers(eventName, false);
                return SdkResponse.CreateSuccess(Newtonsoft.Json.JsonConvert.SerializeObject(events));
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse UpdateEvent(string name, string eventType, bool active = true)
    {
        try
        {
            using (Py.GIL())
            {
                var eventHandler = CreateEventHandler(name, eventType, active);
                _client.EventApi.update_event_handler(eventHandler);
            }
            return SdkResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse DeleteEvent(string name)
    {
        try
        {
            using (Py.GIL())
            {
                _client.EventApi.unregister_event_handler(name);
            }
            return SdkResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    private dynamic CreateEventHandler(string name, string eventType, bool active)
    {
        var eventHandlerModule = Py.Import("conductor.common.metadata.events.event_handler");
        var EventHandler = eventHandlerModule.GetAttr("EventHandler");
        dynamic eventHandler = EventHandler.Invoke();
        
        eventHandler.name = name;
        eventHandler.event_name = eventType;
        eventHandler.active = active;
        eventHandler.actions = new List<dynamic>();
        
        return eventHandler;
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 