using Newtonsoft.Json.Linq;
using Conductor.Api;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.CSharp.Extensions;

namespace SdkTestAutomation.CSharp.Operations;

public static class WorkflowOperations
{
    public static SdkResponse Execute(string operation, Dictionary<string, JToken> parameters)
    {
        return OperationUtils.ExecuteWithErrorHandling(() =>
        {
            var config = OperationUtils.CreateSdkConfiguration();
            var workflowApi = new WorkflowResourceApi(config);
            
            return operation switch
            {
                "get-workflow" => GetWorkflow(parameters, workflowApi),
                _ => throw new ArgumentException($"Unknown workflow operation: {operation}")
            };
        });
    }
    
    private static SdkResponse GetWorkflow(Dictionary<string, JToken> parameters, WorkflowResourceApi workflowApi)
    {
        var workflowId = parameters.GetString("workflowId");
        var workflow = workflowApi.GetWorkflow(workflowId);
        return SdkResponse.CreateSuccess(workflow);
    }
} 