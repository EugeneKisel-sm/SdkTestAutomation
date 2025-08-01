using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public class PythonEventAdapter : BasePythonAdapter, IEventAdapter
{
    public SdkResponse AddEvent(string name, string eventType, bool active = true)
    {
        return ExecutePythonOperation(() =>
        {
            var eventHandler = CreateEventHandler(name, eventType, active);
            _client.EventApi.register_event_handler(eventHandler);
        }, "AddEvent");
    }
    
    public SdkResponse GetEvents()
    {
        return ExecutePythonOperation(() =>
        {
            return _client.EventApi.get_event_handlers("", false);
        }, "GetEvents");
    }
    
    public SdkResponse GetEventByName(string eventName)
    {
        return ExecutePythonOperation(() =>
        {
            return _client.EventApi.get_event_handlers(eventName, false);
        }, "GetEventByName");
    }
    
    public SdkResponse UpdateEvent(string name, string eventType, bool active = true)
    {
        return ExecutePythonOperation(() =>
        {
            var eventHandler = CreateEventHandler(name, eventType, active);
            _client.EventApi.update_event_handler(eventHandler);
        }, "UpdateEvent");
    }
    
    public SdkResponse DeleteEvent(string name)
    {
        return ExecutePythonOperation(() =>
        {
            _client.EventApi.unregister_event_handler(name);
        }, "DeleteEvent");
    }
    
    private dynamic CreateEventHandler(string name, string eventType, bool active)
    {
        try
        {
            dynamic eventHandler = CreatePythonObject("conductor.common.metadata.events.event_handler", "EventHandler");
            
            SetPythonProperty(eventHandler, "name", name);
            SetPythonProperty(eventHandler, "event_name", eventType);
            SetPythonProperty(eventHandler, "active", active);
            SetPythonProperty(eventHandler, "actions", new List<dynamic>());
            
            return eventHandler;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Python EventHandler: {ex.Message}", ex);
        }
    }
} 