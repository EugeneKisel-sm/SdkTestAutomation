using RestSharp;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Response;
using SdkTestAutomation.Utils.Logging;
using HttpClient = SdkTestAutomation.Core.HttpClient;

namespace SdkTestAutomation.Api.Conductor.WorkflowResource;

public class WorkflowResourceApi : HttpClient
{
    public WorkflowResourceApi(ILogger logger) : base(logger)
    {
    }
    
    public RestResponse<GetWorkflowResponse> GetWorkflow(GetWorkflowRequest request, string workflowId)
    {
        return SendGetRequest<GetWorkflowResponse>(ApiUrl.WorkflowResource.WorkflowUrl + $"/{workflowId}", request);
    }
}
