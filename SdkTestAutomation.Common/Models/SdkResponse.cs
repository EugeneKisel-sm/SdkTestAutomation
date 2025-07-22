using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Standardized response structure for all SDK operations
/// </summary>
public class SdkResponse<T>
{
    [JsonProperty("statusCode")]
    public int StatusCode { get; set; }
    
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("data")]
    public T? Data { get; set; }
    
    [JsonProperty("content")]
    public string Content { get; set; } = string.Empty;
    
    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;
    
    [JsonProperty("requestId")]
    public string RequestId { get; set; } = string.Empty;
    
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Non-generic response for operations that don't return data
/// </summary>
public class SdkResponse : SdkResponse<object>
{
}

/// <summary>
/// Response for event operations
/// </summary>
public class GetEventResponse
{
    [JsonProperty("events")]
    public List<EventInfo> Events { get; set; } = new();
    
    [JsonProperty("count")]
    public int Count => Events.Count;
}

/// <summary>
/// Event information model
/// </summary>
public class EventInfo
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;
    
    [JsonProperty("active")]
    public bool Active { get; set; }
    
    [JsonProperty("actions")]
    public List<EventAction> Actions { get; set; } = new();
    
    [JsonProperty("condition")]
    public string? Condition { get; set; }
    
    [JsonProperty("evaluatorType")]
    public string? EvaluatorType { get; set; }
}

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