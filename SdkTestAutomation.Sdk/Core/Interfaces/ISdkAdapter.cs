namespace SdkTestAutomation.Sdk.Core.Interfaces;

public interface ISdkAdapter : IDisposable
{
    string SdkType { get; }
    bool Initialize(string serverUrl);
}