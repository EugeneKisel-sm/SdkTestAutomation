using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;
using Python.Runtime;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public class PythonTokenAdapter : ITokenAdapter
{
    private PythonClient _client;
    
    public string SdkType => "python";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new PythonClient();
            _client.Initialize(serverUrl);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public SdkResponse GenerateToken(string keyId, string keySecret)
    {
        try
        {
            if (_client == null)
            {
                return SdkResponse.CreateError("Python client is not initialized");
            }
            
            using (Py.GIL())
            {
                var generateTokenRequest = CreateGenerateTokenRequest(keyId, keySecret);
                var token = _client.TokenApi.generate_token(generateTokenRequest);
                return SdkResponse.CreateSuccess(token);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse GetUserInfo()
    {
        try
        {
            using (Py.GIL())
            {
                var userInfo = _client.TokenApi.get_user_info();
                return SdkResponse.CreateSuccess(userInfo);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    private dynamic CreateGenerateTokenRequest(string keyId, string keySecret)
    {
        try
        {
            using (Py.GIL())
            {
                // Import the GenerateTokenRequest class using correct module path
                dynamic requestModule = Py.Import("conductor.client.http.models.generate_token_request");
                dynamic GenerateTokenRequest = requestModule.GenerateTokenRequest;
                
                // Create GenerateTokenRequest instance with constructor parameters
                dynamic request = GenerateTokenRequest(keyId, keySecret);
                
                return request;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Python GenerateTokenRequest: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 