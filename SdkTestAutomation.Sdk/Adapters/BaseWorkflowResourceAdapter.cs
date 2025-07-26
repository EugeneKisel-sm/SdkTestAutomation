using SdkTestAutomation.Sdk.Interfaces;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Response;

namespace SdkTestAutomation.Sdk.Adapters;

public abstract class BaseWorkflowResourceAdapter : BaseConductorAdapter, IWorkflowResourceAdapter
{
    public abstract SdkResponse<GetWorkflowResponse> GetWorkflow(GetWorkflowRequest request);
} 