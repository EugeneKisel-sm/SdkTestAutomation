using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Go.Adapters;

public class GoEventAdapter : BaseGoAdapter, IEventAdapter
{
    public SdkResponse AddEvent(string name, string eventType, bool active = true)
    {
        var requestData = new { Name = name, Event = eventType, Active = active };
        return ExecuteGoOperation(
            (data) => _client.ExecuteGoCall("AddEvent", data),
            requestData,
            "AddEvent"
        );
    }
    
    public SdkResponse GetEvents()
    {
        return ExecuteGoOperation(
            () => _client.ExecuteGoCall("GetEvents"),
            "GetEvents"
        );
    }
    
    public SdkResponse GetEventByName(string eventName)
    {
        var requestData = new { EventName = eventName };
        return ExecuteGoOperation(
            (data) => _client.ExecuteGoCall("GetEventByName", data),
            requestData,
            "GetEventByName"
        );
    }
    
    public SdkResponse UpdateEvent(string name, string eventType, bool active = true)
    {
        var requestData = new { Name = name, Event = eventType, Active = active };
        return ExecuteGoOperation(
            (data) => _client.ExecuteGoCall("UpdateEvent", data),
            requestData,
            "UpdateEvent"
        );
    }
    
    public SdkResponse DeleteEvent(string name)
    {
        var requestData = new { Name = name };
        return ExecuteGoOperation(
            (data) => _client.ExecuteGoCall("DeleteEvent", data),
            requestData,
            "DeleteEvent"
        );
    }
} 