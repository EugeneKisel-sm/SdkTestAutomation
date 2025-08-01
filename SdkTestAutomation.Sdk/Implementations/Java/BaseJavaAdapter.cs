using System.Net;
using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java;

public abstract class BaseJavaAdapter
{
    protected readonly BaseJavaClient _client;
    
    protected BaseJavaAdapter(BaseJavaClient client)
    {
        _client = client;
    }
    
    public string SdkType => "java";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client.Initialize(serverUrl);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    protected SdkResponse ExecuteCall(string resource, string operation, object requestData = null)
    {
        try
        {
            var response = Task.Run(() => _client.ExecuteJavaCall(resource, operation, requestData)).Result;
            
            if (string.IsNullOrEmpty(response))
            {
                return SdkResponse.CreateError("Java process returned empty response");
            }
            
            try
            {
                var result = JsonSerializer.Deserialize<JavaResponse>(response);
                
                if (result.Success)
                {
                    return SdkResponse.CreateSuccess(result.Content);
                }
                else
                {
                    return SdkResponse.CreateError(result.Error);
                }
            }
            catch (JsonException jsonEx)
            {
                return SdkResponse.CreateError($"Failed to parse Java response for {resource}.{operation}: {jsonEx.Message}");
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Java SDK call failed for {resource}.{operation}: {ex.Message}");
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
    
} 