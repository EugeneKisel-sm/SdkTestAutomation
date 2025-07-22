using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;

namespace SdkTestAutomation.Java.Commands;

public static class WorkflowCommands
{
    public static string BuildGetWorkflowCommand(GetWorkflowRequest request, string workflowId)
    {
        return $"workflow get --id \"{workflowId}\"";
    }
} 