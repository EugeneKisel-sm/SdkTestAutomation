using System.Net;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Response;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.WorkflowResource;

public class GetWorkflowTests : BaseTest
{
    [Fact]
    public async Task WorkflowResource_GetWorkflow_EmptyName_404()
    {
        var sdkResponse = await ExecuteSdkCallAsync<GetWorkflowResponse>("workflow", new Dictionary<string, object> { }, "get-workflow");
        
        var request = new GetWorkflowRequest();
        var apiResponse = WorkflowResourceApi.GetWorkflow(request, null);
        
        Assert.Equal(HttpStatusCode.NotFound, apiResponse.StatusCode);
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse),
            "SDK response does not match API response");
    }
}