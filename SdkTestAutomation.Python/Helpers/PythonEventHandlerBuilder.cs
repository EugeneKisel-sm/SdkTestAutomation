using Python.Runtime;
using SdkTestAutomation.Api.Conductor.EventResource.Models;

namespace SdkTestAutomation.Python.Helpers;

/// <summary>
/// Helper for building Python event handler objects
/// </summary>
public class PythonEventHandlerBuilder
{
    /// <summary>
    /// Create a Python event handler from request
    /// </summary>
    public dynamic CreateEventHandler(dynamic request)
    {
        using (Py.GIL())
        {
            var eventHandlerModule = Py.Import("conductor.common.metadata.events.event_handler");
            var EventHandler = eventHandlerModule.GetAttr("EventHandler");
            dynamic eventHandler = EventHandler.Invoke();
            
            eventHandler.name = request.Name;
            eventHandler.event_name = request.Event;
            eventHandler.active = request.Active;
            eventHandler.actions = new List<dynamic>();
            
            return eventHandler;
        }
    }
} 