using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java;

public class JavaEventAdapter : IEventAdapter
{
    private JavaClient _client;
    
    public string SdkType => "java";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new JavaClient();
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
            var eventHandler = CreateEventHandler(name, eventType, active);
            _client.EventApi.registerEventHandler(eventHandler);
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
            var events = _client.EventApi.getEventHandlers("", false);
            return SdkResponse.CreateSuccess(Newtonsoft.Json.JsonConvert.SerializeObject(events));
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
            var events = _client.EventApi.getEventHandlers(eventName, false);
            return SdkResponse.CreateSuccess(Newtonsoft.Json.JsonConvert.SerializeObject(events));
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
            var eventHandler = CreateEventHandler(name, eventType, active);
            _client.EventApi.updateEventHandler(eventHandler);
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
            _client.EventApi.unregisterEventHandler(name);
            return SdkResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    private dynamic CreateEventHandler(string name, string eventType, bool active)
    {
        var eventHandler = Activator.CreateInstance(Type.GetType("com.netflix.conductor.common.metadata.events.EventHandler, conductor-common"));
        if (eventHandler != null)
        {
            ((dynamic)eventHandler).setName(name);
            ((dynamic)eventHandler).setEvent(eventType);
            ((dynamic)eventHandler).setActive(active);
        }
        return eventHandler;
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 