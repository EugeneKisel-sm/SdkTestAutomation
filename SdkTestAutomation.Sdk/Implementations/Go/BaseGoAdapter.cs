using System.Net;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Go;

public abstract class BaseGoAdapter
{
    protected readonly GoClient _client;
    
    protected BaseGoAdapter()
    {
        _client = new GoClient();
    }
    
    public string SdkType => "go";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client.Initialize(serverUrl);
            return _client.IsInitialized;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Go adapter: {ex.Message}", ex);
        }
    }

    protected SdkResponse ExecuteGoOperation(Func<string> operation, string operationName)
    {
        try
        {
            if (_client == null || !_client.IsInitialized)
            {
                return SdkResponse.CreateError("Go client is not initialized");
            }
            
            var result = operation();
            return SdkResponse.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"{operationName} failed: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }

    protected SdkResponse ExecuteGoOperation(Func<object, string> operation, object requestData, string operationName)
    {
        try
        {
            if (_client == null || !_client.IsInitialized)
            {
                return SdkResponse.CreateError("Go client is not initialized");
            }
            
            var result = operation(requestData);
            return SdkResponse.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"{operationName} failed: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
    
    public string GetLogs()
    {
        return _client?.GetLogs() ?? string.Empty;
    }
    
    public void ClearLogs()
    {
        _client?.ClearLogs();
    }
} 