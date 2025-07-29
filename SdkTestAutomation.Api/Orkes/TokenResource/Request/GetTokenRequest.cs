using SdkTestAutomation.Core;
using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Api.Orkes.TokenResource.Request;

public class GetTokenRequest : HttpRequest
{
    [Body(Name = "keyId")]
    public string KeyId { get; set; }
    
    [Body(Name = "keySecret")]
    public string KeySecret { get; set; }
}