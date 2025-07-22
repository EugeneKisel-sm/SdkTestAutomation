using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Base class for all SDK requests
/// </summary>
public abstract class SdkRequest
{
    [JsonProperty("requestId")]
    public string RequestId { get; set; } = Guid.NewGuid().ToString();
    
    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Request for adding an event handler
/// </summary>
public class AddEventRequest : SdkRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;
    
    [JsonProperty("active")]
    public bool Active { get; set; } = true;
    
    [JsonProperty("actions")]
    public List<EventAction> Actions { get; set; } = new();
}

/// <summary>
/// Request for getting event handlers
/// </summary>
public class GetEventRequest : SdkRequest
{
    // No additional parameters needed for getting all events
}

/// <summary>
/// Request for getting event handlers by name
/// </summary>
public class GetEventByNameRequest : SdkRequest
{
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;
    
    [JsonProperty("activeOnly")]
    public bool? ActiveOnly { get; set; }
}

/// <summary>
/// Request for updating an event handler
/// </summary>
public class UpdateEventRequest : SdkRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("event")]
    public string Event { get; set; } = string.Empty;
    
    [JsonProperty("active")]
    public bool Active { get; set; } = true;
    
    [JsonProperty("actions")]
    public List<EventAction> Actions { get; set; } = new();
}

/// <summary>
/// Request for deleting an event handler
/// </summary>
public class DeleteEventRequest : SdkRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// Request for getting workflow execution status
/// </summary>
public class GetWorkflowRequest : SdkRequest
{
    [JsonProperty("workflowId")]
    public string WorkflowId { get; set; } = string.Empty;
}

/// <summary>
/// Event action model
/// </summary>
public class EventAction
{
    [JsonProperty("action")]
    public string Action { get; set; } = string.Empty;
    
    [JsonProperty("startWorkflow")]
    public StartWorkflow? StartWorkflow { get; set; }
}

/// <summary>
/// Start workflow model
/// </summary>
public class StartWorkflow
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("version")]
    public int Version { get; set; } = 1;
    
    [JsonProperty("input")]
    public Dictionary<string, object> Input { get; set; } = new();
} 