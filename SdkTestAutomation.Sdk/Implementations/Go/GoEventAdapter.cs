using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;
using System.Text.Json;

namespace SdkTestAutomation.Sdk.Implementations.Go;

public class GoEventAdapter : IEventAdapter
{
    private GoHttpClient _client;
    
    public string SdkType => "go";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new GoHttpClient();
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
            var requestData = new
            {
                name = name,
                @event = eventType,
                active = active
            };
            
            var result = _client.ExecuteGoApiCallAsync("events/add", requestData).Result;
            return SdkResponse.CreateSuccess(result);
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
            var result = _client.ExecuteGoApiCallAsync("events/get").Result;
            return SdkResponse.CreateSuccess(result);
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
            var requestData = new
            {
                eventName = eventName
            };
            
            var result = _client.ExecuteGoApiCallAsync("events/getByName", requestData).Result;
            return SdkResponse.CreateSuccess(result);
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
            var requestData = new
            {
                name = name,
                @event = eventType,
                active = active
            };
            
            var result = _client.ExecuteGoApiCallAsync("events/update", requestData).Result;
            return SdkResponse.CreateSuccess(result);
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
            var requestData = new
            {
                name = name
            };
            
            var result = _client.ExecuteGoApiCallAsync("events/delete", requestData).Result;
            return SdkResponse.CreateSuccess(result);
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