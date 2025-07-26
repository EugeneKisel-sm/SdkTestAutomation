namespace SdkTestAutomation.Sdk.Models;

public class AdapterInfo
{
    public string SdkType { get; set; }
    public string Version { get; set; }
    public bool IsInitialized { get; set; }
    public DateTime InitializedAt { get; set; }
    public string ErrorMessage { get; set; }
} 