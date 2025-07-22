namespace SdkTestAutomation.Api.Conductor.EventResource.Models;

public class StartWorkflow
{
    public string Name { get; set; }
    public int Version { get; set; }
    public string CorrelationId { get; set; }
    public Dictionary<string, object> Input { get; set; }
    public Dictionary<string, string> TaskToDomain { get; set; }
}
