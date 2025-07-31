using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java.Conductor;

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
            var requestData = new
            {
                name = name,
                eventType = eventType,
                active = active
            };
            
            var response = _client.ExecuteJavaCall("event", "add-event", requestData);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
            }
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
            var response = _client.ExecuteJavaCall("event", "get-event", null);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
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
            var requestData = new
            {
                eventName = eventName
            };
            
            var response = _client.ExecuteJavaCall("event", "get-event-by-name", requestData);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
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
            var requestData = new
            {
                name = name,
                eventType = eventType,
                active = active
            };
            
            var response = _client.ExecuteJavaCall("event", "update-event", requestData);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
            }
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
            
            var response = _client.ExecuteJavaCall("event", "delete-event", requestData);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
            }
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
    
    private class JavaResponse
    {
        public bool Success { get; set; }
        public string Data { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
} 