using SdkTestAutomation.Sdk.Core.Interfaces;

namespace SdkTestAutomation.Sdk.Implementations.Java.Orkes;

public class JavaClient : ISdkClient
{
    private dynamic _conductorClient;
    public dynamic TokenApi { get; set; }
    private bool _initialized;
    
    public bool IsInitialized => _initialized && _conductorClient != null;
    
    public void Initialize(string serverUrl)
    {
        try
        {
            var conductorClientType = Type.GetType("com.netflix.conductor.client.http.ConductorClient, orkes-client");
            var tokenClientType = Type.GetType("io.orkes.conductor.client.http.TokenResource, orkes-client");
            
            if (conductorClientType == null || tokenClientType == null)
            {
                throw new InvalidOperationException("Required Java types not found. Ensure JAR files are properly referenced.");
            }
            
            _conductorClient = Activator.CreateInstance(conductorClientType, serverUrl);
            TokenApi = Activator.CreateInstance(tokenClientType, _conductorClient);
            
            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Java client: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        _conductorClient = null;
        TokenApi = null;
        _initialized = false;
    }
} 