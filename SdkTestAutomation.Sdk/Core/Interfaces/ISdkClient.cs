namespace SdkTestAutomation.Sdk.Core.Interfaces;

public interface ISdkClient : IDisposable
{
    bool IsInitialized { get; }
    void Initialize(string serverUrl);
} 