using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.CSharp;

public class CSharpTokenAdapter : ITokenAdapter
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

    public SdkResponse GenerateToken(string keyId, string keySecret)
    {
        try
        {
            var generateTokenRequest = new Conductor.Client.Models.GenerateTokenRequest(keyId, keySecret);
            
            var token = _client.TokenApi.GenerateToken(generateTokenRequest);
            return SdkResponse.CreateSuccess(JsonSerializer.Serialize(token));
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse GetUserInfo()
    {
        throw new NotImplementedException();
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 