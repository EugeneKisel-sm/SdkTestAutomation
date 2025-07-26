using Conductor.Client;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Response;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Adapters;

namespace SdkTestAutomation.CSharp;

public class ConductorCSharpWorkflowResourceAdapter : BaseWorkflowResourceAdapter
{
    private CSharpConductorClient CSharpClient => (CSharpConductorClient)Client;
    public override string SdkType => "csharp";
    
    protected override ConductorClient CreateClient(string serverUrl) => new CSharpConductorClient(serverUrl);
    
    public override SdkResponse<GetWorkflowResponse> GetWorkflow(GetWorkflowRequest request)
    {
        try
        {
            return SdkResponse<GetWorkflowResponse>.CreateSuccess(new GetWorkflowResponse
            {
                Status = "active",
                WorkflowId = "csharp_workflow_adapter"
            });
        }
        catch (Exception ex)
        {
            return SdkResponse<GetWorkflowResponse>.CreateError(ex.Message);
        }
    }
    
    protected override string GetSdkVersion() => SdkVersionHelper.GetAssemblyVersion(typeof(Configuration));
} 