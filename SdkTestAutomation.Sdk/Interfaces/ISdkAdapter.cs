using SdkTestAutomation.Sdk.Models;

namespace SdkTestAutomation.Sdk.Interfaces;

public interface ISdkAdapter : IDisposable
{
    string SdkType { get; }
    
    Task<bool> InitializeAsync(AdapterConfiguration config);
    
    bool IsHealthy();
    
    AdapterInfo GetAdapterInfo();
}