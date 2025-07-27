using Conductor.Client.Models;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.CSharp;

public class CSharpEventAdapter : IEventAdapter
{
    private CSharpClient _client;
    
    public string SdkType => "csharp";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new CSharpClient();
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
            var eventHandler = new Conductor.Client.Models.EventHandler();
            eventHandler.Name = name;
            // Try to set the event property using reflection or different approach
            var eventProperty = typeof(Conductor.Client.Models.EventHandler).GetProperty("Event");
            if (eventProperty != null)
                eventProperty.SetValue(eventHandler, eventType);
            eventHandler.Active = active;
            
            _client.EventApi.AddEventHandler(eventHandler);
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
            var events = _client.EventApi.GetEventHandlers();
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
            var events = _client.EventApi.GetEventHandlersForEvent(eventName, false);
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
            var eventHandler = new Conductor.Client.Models.EventHandler();
            eventHandler.Name = name;
            // Try to set the event property using reflection or different approach
            var eventProperty = typeof(Conductor.Client.Models.EventHandler).GetProperty("Event");
            if (eventProperty != null)
                eventProperty.SetValue(eventHandler, eventType);
            eventHandler.Active = active;
            
            _client.EventApi.UpdateEventHandler(eventHandler);
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
            _client.EventApi.RemoveEventHandlerStatus(name);
            return SdkResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 