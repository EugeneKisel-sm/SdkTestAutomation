namespace SdkTestAutomation.Sdk.Adapters;

/// <summary>
/// Base class for Conductor clients to handle common client management
/// </summary>
public abstract class ConductorClient
{
    protected string ServerUrl { get; private set; }
    
    protected ConductorClient(string serverUrl)
    {
        if (string.IsNullOrEmpty(serverUrl))
        {
            throw new ArgumentException("Server URL cannot be empty", nameof(serverUrl));
        }
        
        ServerUrl = serverUrl;
    }
    
    public abstract bool IsInitialized { get; }
    
    public virtual void Dispose()
    {
        OnDispose();
    }
    
    protected abstract void OnDispose();
    
    public void ThrowIfNotInitialized(string clientName = "Client")
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException($"{clientName} not initialized");
        }
    }
} 