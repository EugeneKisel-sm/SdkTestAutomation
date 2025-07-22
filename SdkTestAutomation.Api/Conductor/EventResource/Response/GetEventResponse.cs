using SdkTestAutomation.Api.Conductor.EventResource.Models;

namespace SdkTestAutomation.Api.Conductor.EventResource.Response;

public class GetEventResponse
{
    public string Name { get; set; }
    public string Event { get; set; }
    public string Condition { get; set; }
    public List<EventAction> Actions { get; set; }
    public bool Active { get; set; }
    public bool EvaluatorType { get; set; }
}