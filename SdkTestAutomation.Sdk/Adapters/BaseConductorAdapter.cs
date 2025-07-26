using SdkTestAutomation.Sdk.Interfaces;
using SdkTestAutomation.Sdk.Models;

namespace SdkTestAutomation.Sdk.Adapters;

public abstract class BaseConductorAdapter : ISdkAdapter
{
    protected AdapterConfiguration Config { get; private set; }
    protected ConductorClient Client { get; private set; }
    
    public abstract string SdkType { get; }
    
    public bool Initialize(AdapterConfiguration config)
    {
        try
        {
            Config = config;
            Client = CreateClient(config.ServerUrl);
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public bool IsHealthy() => Client?.IsInitialized == true;
    
    public AdapterInfo GetAdapterInfo() => new()
    {
        SdkType = SdkType,
        Version = GetSdkVersion(),
        IsInitialized = Client?.IsInitialized == true,
        InitializedAt = DateTime.UtcNow
    };
    
    public void Dispose()
    {
        Client?.Dispose();
        Client = null;
    }
    
    protected abstract ConductorClient CreateClient(string serverUrl);
    protected abstract string GetSdkVersion();
} 