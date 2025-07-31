using System.Text.Json;
using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;

namespace SdkTestAutomation.Sdk.Implementations.Java.Conductor;

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
            var requestData = new
            {
                workflowId = workflowId
            };
            
            var response = _client.ExecuteJavaCall("workflow", "get-workflow", requestData);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
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
            var response = _client.ExecuteJavaCall("workflow", "get-workflows", null);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
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
            var requestData = new
            {
                name = name,
                version = version,
                correlationId = correlationId
            };
            
            var response = _client.ExecuteJavaCall("workflow", "start-workflow", requestData);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
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
            var requestData = new
            {
                workflowId = workflowId,
                reason = reason
            };
            
            var response = _client.ExecuteJavaCall("workflow", "terminate-workflow", requestData);
            var result = JsonSerializer.Deserialize<JavaResponse>(response);
            
            if (result.Success)
            {
                return SdkResponse.CreateSuccess(result.Data);
            }
            else
            {
                return SdkResponse.CreateError(result.Error);
            }
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
    
    private class JavaResponse
    {
        public bool Success { get; set; }
        public string Data { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
} 