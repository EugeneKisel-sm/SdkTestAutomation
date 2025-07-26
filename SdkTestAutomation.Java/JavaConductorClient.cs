using SdkTestAutomation.Java.JavaBridge;
using SdkTestAutomation.Sdk.Adapters;

namespace SdkTestAutomation.Java;

public class JavaConductorClient : ConductorClient
{
    private JavaEngine _javaEngine;
    private dynamic _workflowClient;
    private dynamic _eventClient;
    
    public JavaConductorClient(string serverUrl) : base(serverUrl)
    {
        _javaEngine = new JavaEngine();
        _javaEngine.Initialize(new Sdk.Models.AdapterConfiguration { ServerUrl = serverUrl });
        
        var conductorClient = _javaEngine.GetConductorClient();
        
        // Create workflow client using IKVM
        var workflowType = Type.GetType("com.netflix.conductor.client.http.WorkflowClient, conductor-client");
        if (workflowType != null)
        {
            _workflowClient = Activator.CreateInstance(workflowType, conductorClient);
        }
        
        // Create event client using IKVM
        var eventType = Type.GetType("com.netflix.conductor.client.http.EventClient, conductor-client");
        if (eventType != null)
        {
            _eventClient = Activator.CreateInstance(eventType, conductorClient);
        }
    }
    
    public override bool IsInitialized => _javaEngine != null && _workflowClient != null && _eventClient != null;
    
    public dynamic WorkflowClient
    {
        get
        {
            ThrowIfNotInitialized("Workflow client");
            return _workflowClient;
        }
    }
    
    public dynamic EventClient
    {
        get
        {
            ThrowIfNotInitialized("Event client");
            return _eventClient;
        }
    }
    
    protected override void OnDispose()
    {
        _workflowClient = null;
        _eventClient = null;
        _javaEngine?.Dispose();
        _javaEngine = null;
    }
} 