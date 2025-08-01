using Conductor.Client.Models;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.CSharp.Adapters;

public class CSharpWorkflowAdapter : BaseCSharpAdapter, IWorkflowAdapter
{
    public SdkResponse GetWorkflow(string workflowId)
    {
        return ExecuteCSharpOperation(() =>
        {
            return _client.WorkflowApi.GetExecutionStatus(workflowId);
        }, "GetWorkflow");
    }
    
    public SdkResponse GetWorkflows()
    {
        return ExecuteCSharpOperation(() =>
        {
            return _client.WorkflowApi.GetRunningWorkflow("", null, 100, 0);
        }, "GetWorkflows");
    }
    
    public SdkResponse StartWorkflow(string name, int version, string correlationId = null)
    {
        return ExecuteCSharpOperation(() =>
        {
            var startWorkflowRequest = new StartWorkflowRequest
            {
                Name = name,
                Version = version,
                CorrelationId = correlationId
            };
            
            return _client.WorkflowApi.StartWorkflow(startWorkflowRequest);
        }, "StartWorkflow");
    }
    
    public SdkResponse TerminateWorkflow(string workflowId, string reason = null)
    {
        return ExecuteCSharpOperation(() =>
        {
            _client.WorkflowApi.Terminate(workflowId, reason);
        }, "TerminateWorkflow");
    }
} 