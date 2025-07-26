using SdkTestAutomation.Sdk.Models;

namespace SdkTestAutomation.Sdk.Interfaces;

public interface ISdkAdapter : IDisposable
{
    string SdkType { get; }
    
    bool Initialize(AdapterConfiguration config);
    
    bool IsHealthy();
    
    AdapterInfo GetAdapterInfo();
}