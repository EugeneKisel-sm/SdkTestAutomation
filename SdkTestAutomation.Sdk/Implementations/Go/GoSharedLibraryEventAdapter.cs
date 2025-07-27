using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Go;

public class GoSharedLibraryEventAdapter : IEventAdapter
{
    private GoSharedLibraryClient _client;
    
    public string SdkType => "go";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new GoSharedLibraryClient();
            _client.Initialize(serverUrl);
            return _client.IsInitialized;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Go shared library event adapter: {ex.Message}", ex);
        }
    }
    
    public SdkResponse AddEvent(string name, string eventType, bool active = true)
    {
        try
        {
            var requestData = new { Name = name, Event = eventType, Active = active };
            var result = _client.ExecuteGoCall("AddEvent", requestData);
            
            var response = JsonSerializer.Deserialize<GoResponse>(result);
            if (response?.Success == true)
            {
                return SdkResponse.CreateSuccess(result);
            }
            else
            {
                return SdkResponse.CreateError(response?.Error ?? "Unknown error", 500);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to add event: {ex.Message}", 500);
        }
    }
    
    public SdkResponse GetEvents()
    {
        try
        {
            var result = _client.ExecuteGoCall("GetEvents");
            return SdkResponse.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to get events: {ex.Message}", 500);
        }
    }
    
    public SdkResponse GetEventByName(string eventName)
    {
        try
        {
            var requestData = new { EventName = eventName };
            var result = _client.ExecuteGoCall("GetEventByName", requestData);
            return SdkResponse.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to get event by name: {ex.Message}", 500);
        }
    }
    
    public SdkResponse UpdateEvent(string name, string eventType, bool active = true)
    {
        try
        {
            var requestData = new { Name = name, Event = eventType, Active = active };
            var result = _client.ExecuteGoCall("UpdateEvent", requestData);
            
            var response = JsonSerializer.Deserialize<GoResponse>(result);
            if (response?.Success == true)
            {
                return SdkResponse.CreateSuccess(result);
            }
            else
            {
                return SdkResponse.CreateError(response?.Error ?? "Unknown error", 500);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to update event: {ex.Message}", 500);
        }
    }
    
    public SdkResponse DeleteEvent(string name)
    {
        try
        {
            var requestData = new { Name = name };
            var result = _client.ExecuteGoCall("DeleteEvent", requestData);
            
            var response = JsonSerializer.Deserialize<GoResponse>(result);
            if (response?.Success == true)
            {
                return SdkResponse.CreateSuccess(result);
            }
            else
            {
                return SdkResponse.CreateError(response?.Error ?? "Unknown error", 500);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to delete event: {ex.Message}", 500);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
    
    private class GoResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
} 