namespace SdkTestAutomation.Api.Conductor.EventResource.Models;

public class UpdateWorkflow
{
    public string WorkflowId { get; set; }
    public Dictionary<string, object> Variables { get; set; }
    public bool AppendArray { get; set; }
}