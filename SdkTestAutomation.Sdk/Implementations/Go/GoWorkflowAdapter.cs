using System.Net;
using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Go;

public class GoWorkflowAdapter : IWorkflowAdapter
{
    private GoClient _client;
    
    public string SdkType => "go";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new GoClient();
            _client.Initialize(serverUrl);
            return _client.IsInitialized;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Go workflow adapter: {ex.Message}", ex);
        }
    }
    
    public SdkResponse GetWorkflow(string workflowId)
    {
        try
        {
            var requestData = new { WorkflowId = workflowId };
            var result = _client.ExecuteGoCall("GetWorkflow", requestData);
            return SdkResponse.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to get workflow: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
    
    public SdkResponse GetWorkflows()
    {
        try
        {
            var result = _client.ExecuteGoCall("GetWorkflows");
            return SdkResponse.CreateSuccess(result);
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to get workflows: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
    
    public SdkResponse StartWorkflow(string name, int version, string correlationId = null)
    {
        try
        {
            var requestData = new { Name = name, Version = version, CorrelationId = correlationId ?? "" };
            var result = _client.ExecuteGoCall("StartWorkflow", requestData);
            
            var response = JsonSerializer.Deserialize<GoWorkflowResponse>(result);
            if (response?.Success == true)
            {
                return SdkResponse.CreateSuccess(result);
            }
            else
            {
                return SdkResponse.CreateError(response?.Error ?? "Unknown error", HttpStatusCode.InternalServerError);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to start workflow: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
    
    public SdkResponse TerminateWorkflow(string workflowId, string reason = null)
    {
        try
        {
            var requestData = new { WorkflowId = workflowId, Reason = reason ?? "" };
            var result = _client.ExecuteGoCall("TerminateWorkflow", requestData);
            
            var response = JsonSerializer.Deserialize<GoResponse>(result);
            if (response?.Success == true)
            {
                return SdkResponse.CreateSuccess(result);
            }
            else
            {
                return SdkResponse.CreateError(response?.Error ?? "Unknown error", HttpStatusCode.InternalServerError);
            }
        }
        catch (Exception ex)
        {
            return SdkResponse.CreateError($"Failed to terminate workflow: {ex.Message}", HttpStatusCode.InternalServerError);
        }
    }
    
    public void Dispose()
    {
        _client?.Dispose();
    }

    public string GetLogs()
    {
        return _client?.GetLogs() ?? string.Empty;
    }

    public void ClearLogs()
    {
        _client?.ClearLogs();
    }
    
    private class GoResponse
    {
        public bool Success { get; set; }
        public string Error { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
    
    private class GoWorkflowResponse : GoResponse
    {
        public string WorkflowId { get; set; } = string.Empty;
    }
} 