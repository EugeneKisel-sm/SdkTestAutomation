using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Api.Conductor.EventResource.Request;

public class GetEventByNameRequest : BaseRequest
{
    [UrlParameter]
    public string Event { get; set; }
    
    [UrlParameter]
    public bool? ActiveOnly { get; set; }
} 