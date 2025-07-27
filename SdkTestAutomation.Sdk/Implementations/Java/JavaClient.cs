using SdkTestAutomation.Sdk.Core.Interfaces;

namespace SdkTestAutomation.Sdk.Implementations.Java;

public class JavaClient : ISdkClient
{
    private dynamic _conductorClient;
    public dynamic WorkflowApi { get; set; }
    public dynamic EventApi { get; set; }
    private bool _initialized;
    
    public bool IsInitialized => _initialized && _conductorClient != null;
    
    public void Initialize(string serverUrl)
    {
        try
        {
            // Use proper IKVM.NET type resolution for Conductor v4.x
            // Based on conductor-oss/java-sdk repository structure
            var conductorClientType = Type.GetType("com.netflix.conductor.client.http.ConductorClient, conductor-client");
            var workflowClientType = Type.GetType("com.netflix.conductor.client.http.WorkflowClient, conductor-client");
            var eventClientType = Type.GetType("com.netflix.conductor.client.http.EventClient, conductor-client");
            
            if (conductorClientType == null || workflowClientType == null || eventClientType == null)
            {
                throw new InvalidOperationException("Required Java types not found. Ensure JAR files are properly referenced.");
            }
            
            // Create ConductorClient with server URL
            _conductorClient = Activator.CreateInstance(conductorClientType, serverUrl);
            
            // Create API clients using the conductor client instance
            WorkflowApi = Activator.CreateInstance(workflowClientType, _conductorClient);
            EventApi = Activator.CreateInstance(eventClientType, _conductorClient);
            
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
        WorkflowApi = null;
        EventApi = null;
        _initialized = false;
    }
} 