namespace SdkTestAutomation.Sdk.Implementations.Go.Models;

public class TerminateWorkflowRequest
{
    public string WorkflowId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
} 