using Python.Runtime;
using SdkTestAutomation.Python.PythonBridge;
using SdkTestAutomation.Sdk.Adapters;

namespace SdkTestAutomation.Python;

public class PythonConductorClient : ConductorClient
{
    private PythonBridgeEngine _pythonEngine;
    private dynamic _workflowClient;
    private dynamic _eventClient;
    
    public PythonConductorClient(string serverUrl) : base(serverUrl)
    {
        _pythonEngine = new PythonBridgeEngine();
        _pythonEngine.Initialize(new Sdk.Models.AdapterConfiguration { ServerUrl = serverUrl });
        
        _workflowClient = _pythonEngine.ExecuteWithGIL(() =>
        {
            dynamic workflowClient = Py.Import("conductor.client.http.workflow_client");
            dynamic conductor = Py.Import("conductor.client.http.conductor_client");
            var conductorClient = conductor.ConductorClient(ServerUrl);
            return workflowClient.WorkflowClient(conductorClient);
        });
        
        _eventClient = _pythonEngine.ExecuteWithGIL(() =>
        {
            dynamic eventClient = Py.Import("conductor.client.http.event_client");
            dynamic conductor = Py.Import("conductor.client.http.conductor_client");
            var conductorClient = conductor.ConductorClient(ServerUrl);
            return eventClient.EventClient(conductorClient);
        });
    }
    
    public override bool IsInitialized => _pythonEngine != null && _workflowClient != null && _eventClient != null;
    
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
    
    public T ExecuteWithGIL<T>(Func<T> action) => _pythonEngine.ExecuteWithGIL(action);
    public void ExecuteWithGIL(Action action) => _pythonEngine.ExecuteWithGIL(action);
    
    protected override void OnDispose()
    {
        _workflowClient = null;
        _eventClient = null;
        _pythonEngine?.Dispose();
        _pythonEngine = null;
    }
} 