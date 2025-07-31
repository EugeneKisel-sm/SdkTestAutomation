using System.Reflection;
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
            var path = Directory.GetCurrentDirectory();
            // Load the assemblies
            var conductorCommon = Assembly.LoadFrom(path + "/conductor.common.dll");
            var conductorClient = Assembly.LoadFrom(path + "/conductor.client.dll");
            var orkesConductorClient = Assembly.LoadFrom(path + "/orkes.conductor.client.dll");

            var d = orkesConductorClient.GetTypes();
            
            var generateTokenRequestType = Type.GetType("io.orkes.conductor.client.model.GenerateTokenRequest, orkes.conductor.client");
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