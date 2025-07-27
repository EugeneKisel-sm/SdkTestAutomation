using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Core.Interfaces;

public interface IWorkflowAdapter : ISdkAdapter
{
    SdkResponse GetWorkflow(string workflowId);
    SdkResponse GetWorkflows();
    SdkResponse StartWorkflow(string name, int version, string correlationId = null);
    SdkResponse TerminateWorkflow(string workflowId, string reason = null);
} 