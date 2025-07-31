using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java.Orkes;

public class JavaTokenAdapter : BaseJavaAdapter, ITokenAdapter
{
    public JavaTokenAdapter() : base(new JavaClient()) { }

    public SdkResponse GenerateToken(string keyId, string keySecret)
    {
        return ExecuteCall("token", "generate-token", new { keyId, keySecret });
    }

    public SdkResponse GetUserInfo()
    {
        return ExecuteCall("token", "get-user-info", null);
    }
} 