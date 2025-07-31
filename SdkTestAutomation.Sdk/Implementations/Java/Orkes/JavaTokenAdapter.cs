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
            var requestData = new
            {
                keyId = keyId,
                keySecret = keySecret
            };
            
            var response = _client.ExecuteJavaCall("token", "generate-token", requestData);
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

    public SdkResponse GetUserInfo()
    {
        try
        {
            var response = _client.ExecuteJavaCall("token", "get-user-info", null);
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