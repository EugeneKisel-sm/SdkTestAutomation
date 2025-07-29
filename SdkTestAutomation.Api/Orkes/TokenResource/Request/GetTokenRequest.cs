using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SdkTestAutomation.Core;
using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Api.Orkes.TokenResource.Request;

public class GetTokenRequest : HttpRequest
{
    [Body, JsonProperty("keyId")]
    public string KeyId { get; set; }
    
    [Body, JsonProperty("keySecret")]
    public string KeySecret { get; set; }
}