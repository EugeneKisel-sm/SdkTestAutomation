using Newtonsoft.Json;

namespace SdkTestAutomation.Common.Models;

/// <summary>
/// Request for deleting an event handler
/// </summary>
public class DeleteEventRequest : SdkRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
} 