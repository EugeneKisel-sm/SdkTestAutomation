using SdkTestAutomation.Api.Conductor.EventResource.Models;
using SdkTestAutomation.Core.Attributes;

namespace SdkTestAutomation.Api.Conductor.EventResource.Request;

public class AddEventRequest : BaseRequest
{
    [Body]
    public string Name { get; set; }
    
    [Body]
    public string Event { get; set; }
    
    [Body]
    public string Condition { get; set; }
    
    [Body]
    public List<EventAction> Actions { get; set; }
    
    [Body]
    public bool Active { get; set; }
    
    [Body]
    public bool EvaluatorType { get; set; }
} 