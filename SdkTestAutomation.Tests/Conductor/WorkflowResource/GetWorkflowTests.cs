using System.Net;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.WorkflowResource;

public class GetWorkflowTests : BaseTest
{
    [Fact]
    public void WorkflowResource_GetWorkflow_EmptyName_404()
    {
        var request = new GetWorkflowRequest();
        var response = WorkflowResourceApi.GetWorkflow(request, null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}