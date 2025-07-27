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
        _conductorClient = CreateJavaObject("com.netflix.conductor.client.http.ConductorClient", serverUrl);
        WorkflowApi = CreateJavaObject("com.netflix.conductor.client.http.WorkflowClient", _conductorClient);
        EventApi = CreateJavaObject("com.netflix.conductor.client.http.EventClient", _conductorClient);
        _initialized = true;
    }
    
    private dynamic CreateJavaObject(string className, params object[] args)
    {
        var type = Type.GetType($"{className}, conductor-client") ?? 
                  Type.GetType($"{className}, conductor-common");
        
        if (type == null)
            throw new InvalidOperationException($"Could not find type {className}");
        
        return Activator.CreateInstance(type, args);
    }
    
    public void Dispose()
    {
        _conductorClient = null;
        WorkflowApi = null;
        EventApi = null;
        _initialized = false;
    }
} 