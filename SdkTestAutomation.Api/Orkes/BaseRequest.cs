using SdkTestAutomation.Core;
using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Api.Orkes;

public class BaseRequest : HttpRequest
{
    [Header(Name = "Ocp-Apim-Subscription-Key")]
    public string SubscriptionKey { get; set; }
    
}