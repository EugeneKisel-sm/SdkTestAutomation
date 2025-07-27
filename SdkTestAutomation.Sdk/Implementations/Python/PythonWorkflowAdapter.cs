using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;
using Python.Runtime;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public class PythonWorkflowAdapter : IWorkflowAdapter
{
    private PythonClient _client;
    
    public string SdkType => "python";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new PythonClient();
            _client.Initialize(serverUrl);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public SdkResponse GetWorkflow(string workflowId)
    {
        try
        {
            using (Py.GIL())
            {
                var workflow = _client.WorkflowApi.get_execution_status(workflowId);
                return SdkResponse.CreateSuccess(Newtonsoft.Json.JsonConvert.SerializeObject(workflow));
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse GetWorkflows()
    {
        try
        {
            using (Py.GIL())
            {
                var workflows = _client.WorkflowApi.get_running_workflow("", "", 100, 0);
                return SdkResponse.CreateSuccess(Newtonsoft.Json.JsonConvert.SerializeObject(workflows));
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse StartWorkflow(string name, int version, string correlationId = null)
    {
        try
        {
            using (Py.GIL())
            {
                var startWorkflowRequest = CreateStartWorkflowRequest(name, version, correlationId);
                var workflowId = _client.WorkflowApi.start_workflow(startWorkflowRequest);
                return SdkResponse.CreateSuccess(workflowId);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public SdkResponse TerminateWorkflow(string workflowId, string reason = null)
    {
        try
        {
            using (Py.GIL())
            {
                _client.WorkflowApi.terminate(workflowId, reason);
            }
            return SdkResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    private dynamic CreateStartWorkflowRequest(string name, int version, string correlationId)
    {
        try
        {
            using (Py.GIL())
            {
                // Import the StartWorkflowRequest class using correct module path
                // Based on conductor-oss/python-sdk repository structure
                dynamic requestModule = Py.Import("conductor.common.run.start_workflow_request");
                dynamic StartWorkflowRequest = requestModule.GetAttr("StartWorkflowRequest");
                
                // Create StartWorkflowRequest instance
                dynamic request = StartWorkflowRequest.Invoke();
                
                // Set properties using Python attribute assignment
                request.name = name;
                request.version = version;
                if (!string.IsNullOrEmpty(correlationId))
                    request.correlation_id = correlationId;
                
                return request;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Python StartWorkflowRequest: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 