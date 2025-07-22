namespace SdkTestAutomation.Api.Conductor.WorkflowResource.Response;

public class GetWorkflowResponse
{
    public string WorkflowId { get; set; }
    public string Name { get; set; }
    public int Version { get; set; }
    public string Status { get; set; }
    public Dictionary<string, object> Input { get; set; }
    public Dictionary<string, object> Output { get; set; }
} 