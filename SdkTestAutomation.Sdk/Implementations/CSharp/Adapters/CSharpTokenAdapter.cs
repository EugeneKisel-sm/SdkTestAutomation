using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.CSharp.Adapters;

public class CSharpTokenAdapter : BaseCSharpAdapter, ITokenAdapter
{
    public SdkResponse GenerateToken(string keyId, string keySecret)
    {
        return ExecuteCSharpOperation(() =>
        {
            var generateTokenRequest = new Conductor.Client.Models.GenerateTokenRequest(keyId, keySecret);
            return _client.TokenApi.GenerateToken(generateTokenRequest);
        }, "GenerateToken");
    }
    
    public SdkResponse GetUserInfo()
    {
        throw new NotImplementedException();
    }
} 