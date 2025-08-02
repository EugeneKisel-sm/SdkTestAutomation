using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.CSharp;

public abstract class BaseCSharpAdapter
{
    protected readonly CSharpClient _client;
    
    protected BaseCSharpAdapter()
    {
        _client = new CSharpClient();
    }
    
    public string SdkType => "csharp";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            // If already initialized with the same URL, don't re-initialize
            if (_client.IsInitialized)
            {
                return true;
            }
            
            _client.Initialize(serverUrl);
            return _client.IsInitialized;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize C# adapter: {ex.Message}", ex);
        }
    }

    protected SdkResponse ExecuteCSharpOperation(Func<object> operation, string operationName)
    {
        try
        {
            if (_client == null || !_client.IsInitialized)
            {
                return SdkResponse.CreateError("C# client is not initialized");
            }
            
            var result = operation();
            return SdkResponse.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"{operationName} failed: {ex.Message}");
        }
    }
    
    protected SdkResponse ExecuteCSharpOperation(Action operation, string operationName)
    {
        try
        {
            if (_client == null || !_client.IsInitialized)
            {
                return SdkResponse.CreateError("C# client is not initialized");
            }
            
            operation();
            return SdkResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"{operationName} failed: {ex.Message}");
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 