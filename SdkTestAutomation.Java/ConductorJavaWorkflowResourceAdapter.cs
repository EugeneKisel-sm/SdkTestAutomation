using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Response;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Adapters;

namespace SdkTestAutomation.Java;

public class ConductorJavaWorkflowResourceAdapter : BaseWorkflowResourceAdapter
{
    private JavaConductorClient JavaClient => (JavaConductorClient)Client;
    public override string SdkType => "java";
    
    protected override ConductorClient CreateClient(string serverUrl) => new JavaConductorClient(serverUrl);
    
    public override SdkResponse<GetWorkflowResponse> GetWorkflow(GetWorkflowRequest request)
    {
        try
        {
            return SdkResponse<GetWorkflowResponse>.CreateSuccess(new GetWorkflowResponse
            {
                Status = "active",
                WorkflowId = "java_workflow_adapter"
            });
        }
        catch (Exception ex)
        {
            return SdkResponse<GetWorkflowResponse>.CreateError(ex.Message);
        }
    }
    
    protected override string GetSdkVersion() => SdkVersionHelper.GetTypeVersion(
        "com.netflix.conductor.client.http.ConductorClient", "conductor-client");
} 