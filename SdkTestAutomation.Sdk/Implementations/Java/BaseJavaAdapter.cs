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
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            return result.Success 
                ? SdkResponse.CreateSuccess(result.Data) 
                : SdkResponse.CreateError(result.Error);
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