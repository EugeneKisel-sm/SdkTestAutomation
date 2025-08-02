using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.CSharp.Adapters;

public class CSharpEventAdapter : BaseCSharpAdapter, IEventAdapter
{
    public SdkResponse AddEvent(string name, string eventType, bool active = true)
    {
        return ExecuteCSharpOperation(() =>
        {
            var eventHandler = new Conductor.Client.Models.EventHandler
            {
                Name = name,
                _Event = eventType,
                Active = active,
                Actions = new List<Action>()
            };
            
            _client.EventApi.AddEventHandler(eventHandler);
        }, "AddEvent");
    }
    
    public SdkResponse GetEvents()
    {
        try
        {
            if (_client == null || !_client.IsInitialized)
            {
                return SdkResponse.CreateError("C# client is not initialized");
            }
            
            var result = _client.EventApi.GetEventHandlers();
            return SdkResponse.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"GetEvents failed: {ex.Message}");
        }
    }
    
    public SdkResponse GetEventByName(string eventName)
    {
        return ExecuteCSharpOperation(() => _client.EventApi.GetEventHandlersForEvent(eventName, false), "GetEventByName");
    }
    
    public SdkResponse UpdateEvent(string name, string eventType, bool active = true)
    {
        return ExecuteCSharpOperation(() =>
        {
            var eventHandler = new Conductor.Client.Models.EventHandler
            {
                Name = name,
                _Event = eventType,
                Active = active,
                Actions = new List<Action>()
            };
            
            _client.EventApi.UpdateEventHandler(eventHandler);
        }, "UpdateEvent");
    }
    
    public SdkResponse DeleteEvent(string name)
    {
        return ExecuteCSharpOperation(() =>
        {
            _client.EventApi.RemoveEventHandlerStatus(name);
        }, "DeleteEvent");
    }
} 