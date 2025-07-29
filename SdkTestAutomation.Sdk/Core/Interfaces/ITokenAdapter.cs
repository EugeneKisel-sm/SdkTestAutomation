using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Core.Interfaces;

public interface ITokenAdapter : ISdkAdapter
{
    SdkResponse GenerateToken(string keyId, string keySecret);
    SdkResponse GetUserInfo();
} 