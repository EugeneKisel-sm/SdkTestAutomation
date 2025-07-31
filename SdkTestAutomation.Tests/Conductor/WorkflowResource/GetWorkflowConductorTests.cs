using System.Net;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor.WorkflowResource;

public class GetWorkflowConductorTests : BaseConductorTest
{
    [Fact]
    [Trait(TraitName.Category, TestType.Conductor)]
    public void WorkflowResource_GetWorkflow_EmptyName_404()
    {
        var sdkResponse = WorkflowAdapter.GetWorkflow("");

        Assert.False(sdkResponse.Success);
        Assert.Equal(HttpStatusCode.NotFound, sdkResponse.StatusCode);
    }
    
    [Fact]
    [Trait(TraitName.Category, TestType.Conductor)]
    public void WorkflowResource_GetWorkflows_200()
    {
        var sdkResponse = WorkflowAdapter.GetWorkflows();

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, sdkResponse.StatusCode);
    }
}