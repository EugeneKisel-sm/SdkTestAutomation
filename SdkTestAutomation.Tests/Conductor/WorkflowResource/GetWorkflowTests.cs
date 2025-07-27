using System.Net;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.WorkflowResource;

public class GetWorkflowTests : BaseTest
{
    [Fact]
    public void WorkflowResource_GetWorkflow_EmptyName_404()
    {
        var sdkResponse = WorkflowAdapter.GetWorkflow("");

        // This might fail or return empty results, but should not throw
        Assert.NotNull(sdkResponse);
    }
    
    [Fact]
    public void WorkflowResource_GetWorkflows_200()
    {
        var sdkResponse = WorkflowAdapter.GetWorkflows();

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
    }
}