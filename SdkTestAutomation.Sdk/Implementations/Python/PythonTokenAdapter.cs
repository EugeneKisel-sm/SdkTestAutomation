using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public class PythonTokenAdapter : BasePythonAdapter, ITokenAdapter
{
    public SdkResponse GenerateToken(string keyId, string keySecret)
    {
        return ExecutePythonOperation(() =>
        {
            var generateTokenRequest = CreateGenerateTokenRequest(keyId, keySecret);
            return _client.TokenApi.generate_token(generateTokenRequest);
        }, "GenerateToken");
    }
    
    public SdkResponse GetUserInfo()
    {
        return ExecutePythonOperation(() =>
        {
            return _client.TokenApi.get_user_info();
        }, "GetUserInfo");
    }
    
    private dynamic CreateGenerateTokenRequest(string keyId, string keySecret)
    {
        try
        {
            return CreatePythonObject("conductor.client.http.models.generate_token_request", "GenerateTokenRequest", keyId, keySecret);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Python GenerateTokenRequest: {ex.Message}", ex);
        }
    }
} 