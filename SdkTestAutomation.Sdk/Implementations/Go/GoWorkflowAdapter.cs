using SdkTestAutomation.Sdk.Core.Interfaces;
using SdkTestAutomation.Sdk.Core.Models;
using System.Text.Json;

namespace SdkTestAutomation.Sdk.Implementations.Go;

public class GoWorkflowAdapter : IWorkflowAdapter
{
    private GoHttpClient _client;
    
    public string SdkType => "go";
    
    public bool Initialize(string serverUrl)
    {
        try
        {
            _client = new GoHttpClient();
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
            
            var result = _client.ExecuteGoApiCallAsync("workflows/get", requestData).Result;
            return SdkResponse.CreateSuccess(result);
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
            var result = _client.ExecuteGoApiCallAsync("workflows/getAll").Result;
            return SdkResponse.CreateSuccess(result);
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
                correlationId = correlationId ?? ""
            };
            
            var result = _client.ExecuteGoApiCallAsync("workflows/start", requestData).Result;
            return SdkResponse.CreateSuccess(result);
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
                reason = reason ?? ""
            };
            
            var result = _client.ExecuteGoApiCallAsync("workflows/terminate", requestData).Result;
            return SdkResponse.CreateSuccess(result);
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