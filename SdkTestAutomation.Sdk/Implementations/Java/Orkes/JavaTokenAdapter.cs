using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java.Orkes;

public class JavaTokenAdapter : ITokenAdapter
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

    public SdkResponse GenerateToken(string keyId, string keySecret)
    {
        try
        {
            var generateTokenRequestType = Type.GetType("io.orkes.conductor.client.model.GenerateTokenRequest, orkes-client");
            if (generateTokenRequestType == null)
            {
                throw new InvalidOperationException("GenerateTokenRequest type not found in orkes-client assembly");
            }
            
            var generateTokenRequest = Activator.CreateInstance(generateTokenRequestType, keyId, keySecret);
            var token = _client.TokenApi.generateToken(generateTokenRequest);
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