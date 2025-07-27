using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java;

public class JavaWorkflowAdapter : IWorkflowAdapter
{
    private JavaClient _client;
    
    public string SdkType => "java";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new JavaClient();
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
            var workflow = _client.WorkflowApi.getExecutionStatus(workflowId);
            return SdkResponse.CreateSuccess(Newtonsoft.Json.JsonConvert.SerializeObject(workflow));
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
            // Get running workflows with empty strings for name and correlationId, limit 100, offset 0
            var workflows = _client.WorkflowApi.getRunningWorkflow("", "", 100, 0);
            return SdkResponse.CreateSuccess(Newtonsoft.Json.JsonConvert.SerializeObject(workflows));
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
            var startWorkflowRequest = CreateStartWorkflowRequest(name, version, correlationId);
            var workflowId = _client.WorkflowApi.startWorkflow(startWorkflowRequest);
            return SdkResponse.CreateSuccess(workflowId);
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
            _client.WorkflowApi.terminate(workflowId, reason);
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
            // Based on conductor-oss/java-sdk repository structure
            var requestType = Type.GetType("com.netflix.conductor.common.run.StartWorkflowRequest, conductor-common");
            if (requestType == null)
            {
                throw new InvalidOperationException("StartWorkflowRequest type not found in conductor-common assembly");
            }
            
            var request = Activator.CreateInstance(requestType);
            if (request != null)
            {
                ((dynamic)request).setName(name);
                ((dynamic)request).setVersion(version);
                if (!string.IsNullOrEmpty(correlationId))
                    ((dynamic)request).setCorrelationId(correlationId);
            }
            return request;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create StartWorkflowRequest: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 