using System.Text.Json;
using Conductor.Api;
using Conductor.Client;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.CSharp.Extensions;

namespace SdkTestAutomation.CSharp.Operations;

public static class WorkflowOperations
{
    public static SdkResponse Execute(string operation, Dictionary<string, JsonElement> parameters)
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
    
    private static SdkResponse GetWorkflow(Dictionary<string, JsonElement> parameters, WorkflowResourceApi workflowApi)
    {
        var workflowId = parameters.GetString("workflowId");
        var workflow = workflowApi.GetExecutionStatus(workflowId);
        return SdkResponse.CreateSuccess(workflow);
    }
} 