using Conductor.Client.Models;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.CSharp;

public class CSharpWorkflowAdapter : IWorkflowAdapter
{
    private CSharpClient _client;
    
    public string SdkType => "csharp";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new CSharpClient();
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
            var workflow = _client.WorkflowApi.GetExecutionStatus(workflowId);
            return SdkResponse.CreateSuccess(workflow);
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
            var workflows = _client.WorkflowApi.GetRunningWorkflow("", null, 100, 0);
            return SdkResponse.CreateSuccess(workflows);
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
            var startWorkflowRequest = new StartWorkflowRequest
            {
                Name = name,
                Version = version,
                CorrelationId = correlationId
            };
            
            var workflowId = _client.WorkflowApi.StartWorkflow(startWorkflowRequest);
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
            _client.WorkflowApi.Terminate(workflowId, reason);
            return SdkResponse.CreateSuccess();
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError(ex.Message);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }
} 