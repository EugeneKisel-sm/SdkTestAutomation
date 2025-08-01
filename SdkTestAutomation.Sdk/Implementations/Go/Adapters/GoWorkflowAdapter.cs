using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Go.Adapters;

public class GoWorkflowAdapter : BaseGoAdapter, IWorkflowAdapter
{
    public SdkResponse GetWorkflow(string workflowId)
    {
        var requestData = new { WorkflowId = workflowId };
        return ExecuteGoOperation(
            (data) => _client.ExecuteGoCall("GetWorkflow", data),
            requestData,
            "GetWorkflow"
        );
    }
    
    public SdkResponse GetWorkflows()
    {
        return ExecuteGoOperation(
            () => _client.ExecuteGoCall("GetWorkflows"),
            "GetWorkflows"
        );
    }
    
    public SdkResponse StartWorkflow(string name, int version, string correlationId = null)
    {
        var requestData = new { Name = name, Version = version, CorrelationId = correlationId ?? "" };
        return ExecuteGoOperation(
            (data) => _client.ExecuteGoCall("StartWorkflow", data),
            requestData,
            "StartWorkflow"
        );
    }
    
    public SdkResponse TerminateWorkflow(string workflowId, string reason = null)
    {
        var requestData = new { WorkflowId = workflowId, Reason = reason ?? "" };
        return ExecuteGoOperation(
            (data) => _client.ExecuteGoCall("TerminateWorkflow", data),
            requestData,
            "TerminateWorkflow"
        );
    }
} 