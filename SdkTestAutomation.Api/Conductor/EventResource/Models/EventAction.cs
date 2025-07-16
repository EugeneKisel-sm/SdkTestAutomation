namespace SdkTestAutomation.Api.Conductor.EventResource.Models;

public class EventAction
{
    public string Action { get; set; }
    public StartWorkflow StartWorkflow { get; set; }
    public TaskAction CompleteTask { get; set; }
    public TaskAction FailTask { get; set; }
    public TerminateWorkflow TerminateWorkflow { get; set; }
    public UpdateWorkflow UpdateWorkflow { get; set; }
    public bool ExpandInlineJson { get; set; }
}