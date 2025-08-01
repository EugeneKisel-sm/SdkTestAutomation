using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public class PythonWorkflowAdapter : BasePythonAdapter, IWorkflowAdapter
{
    public SdkResponse GetWorkflow(string workflowId)
    {
        return ExecutePythonOperation(() =>
        {
            return _client.WorkflowApi.get_execution_status(workflowId);
        }, "GetWorkflow");
    }
    
    public SdkResponse GetWorkflows()
    {
        return ExecutePythonOperation(() =>
        {
            return _client.WorkflowApi.get_running_workflow("", "", 100, 0);
        }, "GetWorkflows");
    }
    
    public SdkResponse StartWorkflow(string name, int version, string correlationId = null)
    {
        return ExecutePythonOperation(() =>
        {
            var startWorkflowRequest = CreateStartWorkflowRequest(name, version, correlationId);
            return _client.WorkflowApi.start_workflow(startWorkflowRequest);
        }, "StartWorkflow");
    }
    
    public SdkResponse TerminateWorkflow(string workflowId, string reason = null)
    {
        return ExecutePythonOperation(() =>
        {
            _client.WorkflowApi.terminate(workflowId, reason);
        }, "TerminateWorkflow");
    }
    
    private dynamic CreateStartWorkflowRequest(string name, int version, string correlationId)
    {
        try
        {
            dynamic request = CreatePythonObject("conductor.common.run.start_workflow_request", "StartWorkflowRequest");
            
            SetPythonProperty(request, "name", name);
            SetPythonProperty(request, "version", version);
            if (!string.IsNullOrEmpty(correlationId))
                SetPythonProperty(request, "correlation_id", correlationId);
            
            return request;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Python StartWorkflowRequest: {ex.Message}", ex);
        }
    }
} 