using SdkTestAutomation.Core;
using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Api.Orkes.TokenResource.Request;

public class GetTokenRequest : HttpRequest
{
    [Body]
    public string KeyId { get; set; }
    
    [Body]
    public string KeySecret { get; set; }
}