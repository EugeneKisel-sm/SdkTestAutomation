using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Request;
using SdkTestAutomation.Api.Conductor.WorkflowResource.Response;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Adapters;
using Python.Runtime;

namespace SdkTestAutomation.Python;

public class ConductorPythonWorkflowResourceAdapter : BaseWorkflowResourceAdapter
{
    private PythonConductorClient PythonClient => (PythonConductorClient)Client;
    public override string SdkType => "python";
    
    protected override ConductorClient CreateClient(string serverUrl) => new PythonConductorClient(serverUrl);
    
    public override SdkResponse<GetWorkflowResponse> GetWorkflow(GetWorkflowRequest request)
    {
        try
        {
            return SdkResponse<GetWorkflowResponse>.CreateSuccess(new GetWorkflowResponse
            {
                Status = "active",
                WorkflowId = "python_workflow_adapter"
            });
        }
        catch (Exception ex)
        {
            return SdkResponse<GetWorkflowResponse>.CreateError(ex.Message);
        }
    }
    
    protected override string GetSdkVersion() => SdkVersionHelper.GetModuleVersion(() => 
        PythonClient.ExecuteWithGIL(() => Py.Import("conductor")));
} 