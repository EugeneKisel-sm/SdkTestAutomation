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
        try
        {
            if (!PythonEngine.IsInitialized)
            {
                PythonEngine.Initialize();
            }
            
            using (Py.GIL())
            {
                // Import required modules using correct paths from conductor-python SDK
                // Based on conductor-oss/python-sdk repository structure
                dynamic conductor = Py.Import("conductor.client.http.conductor_client");
                dynamic workflowClient = Py.Import("conductor.client.http.workflow_client");
                dynamic eventClient = Py.Import("conductor.client.http.event_client");
                
                // Create client instances
                _conductorClient = conductor.ConductorClient(serverUrl);
                WorkflowApi = workflowClient.WorkflowClient(_conductorClient);
                EventApi = eventClient.EventClient(_conductorClient);
            }
            
            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Python client: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        if (_initialized)
        {
            try
            {
                using (Py.GIL())
                {
                    _conductorClient = null;
                    WorkflowApi = null;
                    EventApi = null;
                }
            }
            catch (Exception ex)
            {
                // Log disposal error but don't throw
                System.Diagnostics.Debug.WriteLine($"Error during Python client disposal: {ex.Message}");
            }
        }
        _initialized = false;
    }
} 