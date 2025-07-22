using System.Net;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using SdkTestAutomation.Common.Configuration;
using Xunit;

namespace SdkTestAutomation.Tests.Conductor;

public class WorkflowResourceSdkTests : BaseSdkTest
{
    [Fact]
    [Trait("Category", "SdkComparison")]
    public Task GetWorkflow_CompareSdkWithRestApi_ShouldMatch()
    {
        var workflowId = "test-workflow-id";
        var request = new GetWorkflowRequest();
        
        var sdkCommand = $"workflow get --id \"{workflowId}\"";
        
        return ValidateSdkResponseAsync(
            sdkCommand: sdkCommand,
            sdkArgs: "",
            restApiCall: async () => WorkflowResourceApi.GetWorkflow(request, workflowId),
            testDescription: $"Get workflow '{workflowId}' comparison test");
    }
    
    [Fact]
    [Trait("Category", "SdkComparison")]
    public Task GetWorkflowWithComplexId_CompareSdkWithRestApi_ShouldMatch()
    {
        var workflowId = $"complex-workflow-{Guid.NewGuid():N}";
        var request = new GetWorkflowRequest();
        
        var sdkCommand = $"workflow get --id \"{workflowId}\"";
        
        return ValidateSdkResponseAsync(
            sdkCommand: sdkCommand,
            sdkArgs: "",
            restApiCall: async () => WorkflowResourceApi.GetWorkflow(request, workflowId),
            testDescription: $"Get complex workflow '{workflowId}' comparison test");
    }
} 