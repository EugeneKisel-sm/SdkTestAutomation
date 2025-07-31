using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java.Conductor;

public class JavaWorkflowAdapter : BaseJavaAdapter, IWorkflowAdapter
{
    public JavaWorkflowAdapter() : base(new JavaClient()) { }
    
    public SdkResponse GetWorkflow(string workflowId)
    {
        return ExecuteCall("workflow", "get-workflow", new { workflowId });
    }
    
    public SdkResponse GetWorkflows()
    {
        return ExecuteCall("workflow", "get-workflows", null);
    }
    
    public SdkResponse StartWorkflow(string name, int version, string correlationId = null)
    {
        return ExecuteCall("workflow", "start-workflow", new { name, version, correlationId });
    }
    
    public SdkResponse TerminateWorkflow(string workflowId, string reason = null)
    {
        return ExecuteCall("workflow", "terminate-workflow", new { workflowId, reason });
    }
} 