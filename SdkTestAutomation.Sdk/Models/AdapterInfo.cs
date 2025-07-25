namespace SdkTestAutomation.Sdk.Models;

/// <summary>
/// Information about an SDK adapter
/// </summary>
public class AdapterInfo
{
    public string SdkType { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public bool IsInitialized { get; set; }
    public DateTime InitializedAt { get; set; }
    public string ErrorMessage { get; set; }
} 