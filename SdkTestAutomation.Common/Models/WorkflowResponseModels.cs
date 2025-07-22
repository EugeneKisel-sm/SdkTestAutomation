using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Response for workflow operations
/// </summary>
public class GetWorkflowResponse
{
    [JsonProperty("workflowId")]
    public string WorkflowId { get; set; } = string.Empty;
    
    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;
    
    [JsonProperty("startTime")]
    public DateTime? StartTime { get; set; }
    
    [JsonProperty("endTime")]
    public DateTime? EndTime { get; set; }
    
    [JsonProperty("input")]
    public Dictionary<string, object> Input { get; set; } = new();
    
    [JsonProperty("output")]
    public Dictionary<string, object> Output { get; set; } = new();
} 