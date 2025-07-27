using SdkTestAutomation.Sdk.Core.Interfaces;
using Python.Runtime;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public class PythonClient : ISdkClient
{
    private dynamic _conductorClient;
    public dynamic WorkflowApi { get; set; }
    public dynamic EventApi { get; set; }
    private bool _initialized;
    
    public bool IsInitialized => _initialized && _conductorClient != null;
    
    public void Initialize(string serverUrl)
    {
        if (!PythonEngine.IsInitialized)
            PythonEngine.Initialize();
        
        using (Py.GIL())
        {
            dynamic conductor = Py.Import("conductor.client.http.conductor_client");
            dynamic workflowClient = Py.Import("conductor.client.http.workflow_client");
            dynamic eventClient = Py.Import("conductor.client.http.event_client");
            
            _conductorClient = conductor.ConductorClient(serverUrl);
            WorkflowApi = workflowClient.WorkflowClient(_conductorClient);
            EventApi = eventClient.EventClient(_conductorClient);
        }
        
        _initialized = true;
    }
    
    public void Dispose()
    {
        if (_initialized)
        {
            using (Py.GIL())
            {
                _conductorClient = null;
                WorkflowApi = null;
                EventApi = null;
            }
        }
        _initialized = false;
    }
} 