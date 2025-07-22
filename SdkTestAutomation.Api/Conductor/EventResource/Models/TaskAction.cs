namespace SdkTestAutomation.Api.Conductor.EventResource.Models;

public class TaskAction
{
    public string WorkflowId { get; set; }
    public string TaskRefName { get; set; }
    public Dictionary<string, object> Output { get; set; }
    public string TaskId { get; set; }
}
