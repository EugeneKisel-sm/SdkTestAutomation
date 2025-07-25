using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Response;

namespace SdkTestAutomation.Sdk.Interfaces;

public interface IWorkflowResourceAdapter : ISdkAdapter
{
    SdkResponse<GetWorkflowResponse> GetWorkflow(GetWorkflowRequest request);
} 