namespace SdkTestAutomation.Sdk.Implementations.Go.Models;

public class StartWorkflowRequest
{
    public string Name { get; set; } = string.Empty;
    public int Version { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
} 