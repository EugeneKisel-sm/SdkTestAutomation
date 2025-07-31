using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java.Conductor;

public class JavaEventAdapter : BaseJavaAdapter, IEventAdapter
{
    public JavaEventAdapter() : base(new JavaClient()) { }
    
    public SdkResponse AddEvent(string name, string eventType, bool active = true)
    {
        return ExecuteCall("event", "add-event", new { name, eventType, active });
    }
    
    public SdkResponse GetEvents()
    {
        return ExecuteCall("event", "get-event", null);
    }
    
    public SdkResponse GetEventByName(string eventName)
    {
        return ExecuteCall("event", "get-event-by-name", new { eventName });
    }
    
    public SdkResponse UpdateEvent(string name, string eventType, bool active = true)
    {
        return ExecuteCall("event", "update-event", new { name, eventType, active });
    }
    
    public SdkResponse DeleteEvent(string name)
    {
        return ExecuteCall("event", "delete-event", new { name });
    }
} 