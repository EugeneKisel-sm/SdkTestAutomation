using SdkTestAutomation.Sdk.Core.Interfaces;
using Python.Runtime;

namespace SdkTestAutomation.Sdk.Implementations.Python;

public class PythonClient : ISdkClient
{
    private dynamic _conductorClient;
    public dynamic WorkflowApi { get; set; }
    public dynamic EventApi { get; set; }
    public dynamic TokenApi { get; set; }
    private bool _initialized;
    
    public bool IsInitialized => _initialized && _conductorClient != null;
    
    public void Initialize(string serverUrl)
    {
        try
        {
            if (!PythonEngine.IsInitialized)
            {
                // Set the Python DLL path - this is REQUIRED for Python.NET 3.0+
                // Use Python 3.10 which is supported by Python.NET 3.0.0
                var pythonDllPath = "/opt/homebrew/Cellar/python@3.10/3.10.18/Frameworks/Python.framework/Versions/3.10/lib/libpython3.10.dylib";
                var pythonHome = "/opt/homebrew/Cellar/python@3.10/3.10.18/Frameworks/Python.framework/Versions/3.10";
                
                if (File.Exists(pythonDllPath))
                {
                    Runtime.PythonDLL = pythonDllPath;
                    Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome);
                }
                else
                {
                    throw new InvalidOperationException("Could not find Python 3.10 shared library");
                }
                
                // Set PYTHONPATH to include the conductor-python-env site-packages
                // The conductor-python-env is in the project root, not in the current directory
                var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../.."));
                var conductorPythonEnvPath = Path.Combine(projectRoot, "conductor-python-env");
                var sitePackagesPath = Path.Combine(conductorPythonEnvPath, "lib", "python3.10", "site-packages");
                
                if (Directory.Exists(sitePackagesPath))
                {
                    Environment.SetEnvironmentVariable("PYTHONPATH", sitePackagesPath);
                }
                
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
            }
            
            using (Py.GIL())
            {
                // Add the site-packages path to Python's sys.path
                dynamic sys = Py.Import("sys");
                var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../.."));
                var sitePackagesPath = Path.Combine(projectRoot, "conductor-python-env", "lib", "python3.10", "site-packages");
                sys.path.append(sitePackagesPath);
                
                // Import required modules using correct paths from conductor-python SDK
                dynamic apiClient = Py.Import("conductor.client.http.api_client");
                dynamic configuration = Py.Import("conductor.client.configuration.configuration");
                dynamic tokenResourceApi = Py.Import("conductor.client.http.api.token_resource_api");
                dynamic eventResourceApi = Py.Import("conductor.client.http.api.event_resource_api");
                dynamic workflowResourceApi = Py.Import("conductor.client.http.api.workflow_resource_api");
                
                // Create configuration with base URL (remove /api suffix if present)
                var baseUrl = serverUrl;
                if (baseUrl.EndsWith("/api"))
                {
                    baseUrl = baseUrl.Substring(0, baseUrl.Length - 4);
                }
                dynamic config = configuration.Configuration(baseUrl);
                
                // Create API client with configuration
                _conductorClient = apiClient.ApiClient(config);
                
                // Create resource API instances
                TokenApi = tokenResourceApi.TokenResourceApi(_conductorClient);
                EventApi = eventResourceApi.EventResourceApi(_conductorClient);
                WorkflowApi = workflowResourceApi.WorkflowResourceApi(_conductorClient);
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
                    TokenApi = null;
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