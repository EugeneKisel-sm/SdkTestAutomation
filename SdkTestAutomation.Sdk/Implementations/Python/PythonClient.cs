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
                InitializePythonEngine();
            }
            
            using (Py.GIL())
            {
                SetupPythonPath();
                InitializeConductorClient(serverUrl);
            }
            
            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to initialize Python client: {ex.Message}", ex);
        }
    }
    
    private void InitializePythonEngine()
    {
        // Set the Python DLL path - this is REQUIRED for Python.NET 3.0+
        // Use Python 3.11 which is supported by Python.NET 3.0.5
        var pythonDllPath = "/opt/homebrew/Cellar/python@3.11/3.11.13/Frameworks/Python.framework/Versions/3.11/lib/libpython3.11.dylib";
        var pythonHome = "/opt/homebrew/Cellar/python@3.11/3.11.13/Frameworks/Python.framework/Versions/3.11";
        
        if (!File.Exists(pythonDllPath))
        {
            throw new InvalidOperationException("Could not find Python 3.11 shared library");
        }
        
        Runtime.PythonDLL = pythonDllPath;
        Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome);
        
        // Set PYTHONPATH to include the conductor-python-env site-packages
        var sitePackagesPath = GetSitePackagesPath();
        if (Directory.Exists(sitePackagesPath))
        {
            Environment.SetEnvironmentVariable("PYTHONPATH", sitePackagesPath);
        }
        
        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
    }
    
    private void SetupPythonPath()
    {
        // Add the site-packages path to Python's sys.path
        dynamic sys = Py.Import("sys");
        var sitePackagesPath = GetSitePackagesPath();
        sys.path.append(sitePackagesPath);
    }
    
    private string GetSitePackagesPath()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../.."));
        var conductorPythonEnvPath = Path.Combine(projectRoot, "conductor-python-env");
        return Path.Combine(conductorPythonEnvPath, "lib", "python3.11", "site-packages");
    }
    
    private void InitializeConductorClient(string serverUrl)
    {
        dynamic apiClient = Py.Import("conductor.client.http.api_client");
        dynamic configuration = Py.Import("conductor.client.configuration.configuration");
        dynamic tokenResourceApi = Py.Import("conductor.client.http.api.token_resource_api");
        dynamic eventResourceApi = Py.Import("conductor.client.http.api.event_resource_api");
        dynamic workflowResourceApi = Py.Import("conductor.client.http.api.workflow_resource_api");
        
        var baseUrl = serverUrl;
        if (baseUrl.EndsWith("/api"))
        {
            baseUrl = baseUrl.Substring(0, baseUrl.Length - 4);
        }
        dynamic config = configuration.Configuration(baseUrl);
        
        _conductorClient = apiClient.ApiClient(config);
        
        TokenApi = tokenResourceApi.TokenResourceApi(_conductorClient);
        EventApi = eventResourceApi.EventResourceApi(_conductorClient);
        WorkflowApi = workflowResourceApi.WorkflowResourceApi(_conductorClient);
    }
    
    public void Dispose()
    {
        if (!_initialized) return;
        
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
            System.Diagnostics.Debug.WriteLine($"Error during Python client disposal: {ex.Message}");
        }
        finally
        {
            _initialized = false;
        }
    }
} 