using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Request for getting workflow execution status
/// </summary>
public class GetWorkflowRequest : SdkRequest
{
    [JsonProperty("workflowId")]
    public string WorkflowId { get; set; } = string.Empty;
} 