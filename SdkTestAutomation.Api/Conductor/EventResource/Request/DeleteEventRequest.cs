using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Api.Conductor.EventResource.Request;

public class DeleteEventRequest : BaseRequest
{
    [UrlParameter]
    public string Name { get; set; }
} 