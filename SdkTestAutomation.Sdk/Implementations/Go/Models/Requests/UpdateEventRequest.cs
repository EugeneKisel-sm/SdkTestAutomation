namespace SdkTestAutomation.Sdk.Implementations.Go.Models;

public class UpdateEventRequest
{
    public string Name { get; set; } = string.Empty;
    public string Event { get; set; } = string.Empty;
    public bool Active { get; set; } = true;
} 