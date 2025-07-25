using System.Reflection;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils.Logging;
using Python.Runtime;

namespace SdkTestAutomation.Python.PythonBridge;

/// <summary>
/// Python bridge engine using pythonnet
/// </summary>
public class PythonBridgeEngine : IDisposable
{
    private static readonly ILogger _logger = new ConsoleLogger(null);
    private bool _initialized = false;
    private bool _disposed = false;
    private dynamic _conductorClient;
    private dynamic _eventClient;
    
    /// <summary>
    /// Initialize the Python bridge
    /// </summary>
    public void Initialize(AdapterConfiguration config)
    {
        if (_initialized) return;
        
        try
        {
            _logger.Log("Initializing Python bridge...");
            
            // Set Python environment
            if (!string.IsNullOrEmpty(config.PythonHome))
            {
                Environment.SetEnvironmentVariable("PYTHONHOME", config.PythonHome);
                _logger.Log($"Set PYTHONHOME to: {config.PythonHome}");
            }
            
            if (!string.IsNullOrEmpty(config.PythonPath))
            {
                Environment.SetEnvironmentVariable("PYTHONPATH", config.PythonPath);
                _logger.Log($"Set PYTHONPATH to: {config.PythonPath}");
            }
            
            // Check for virtual environment path
            var venvPath = Environment.GetEnvironmentVariable("PYTHON_VENV_PATH");
            if (!string.IsNullOrEmpty(venvPath))
            {
                var venvPythonPath = Path.Combine(venvPath, "lib", "python3.13", "site-packages");
                if (Directory.Exists(venvPythonPath))
                {
                    Environment.SetEnvironmentVariable("PYTHONPATH", venvPythonPath);
                    _logger.Log($"Set PYTHONPATH to virtual environment: {venvPythonPath}");
                }
            }
            
            // Initialize Python runtime using direct Python.NET API
            if (!PythonEngine.IsInitialized)
            {
                PythonEngine.Initialize();
                _logger.Log("Python runtime initialized");
            }
            
            // Import required Python modules using direct Python.NET API
            using (Py.GIL())
            {
                // Import conductor SDK
                dynamic conductor = Py.Import("conductor.client.http.conductor_client");
                dynamic eventClient = Py.Import("conductor.client.http.event_client");
                
                // Create Conductor client
                _conductorClient = conductor.ConductorClient(config.ServerUrl);
                _eventClient = eventClient.EventClient(_conductorClient);
                
                _logger.Log("Python Conductor client created successfully");
            }
            
            _initialized = true;
            _logger.Log("Python bridge initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.Log($"Failed to initialize Python bridge: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Get the event client
    /// </summary>
    public dynamic GetEventClient()
    {
        if (!_initialized)
            throw new InvalidOperationException("Python bridge not initialized");
        
        return _eventClient!;
    }
    
    /// <summary>
    /// Execute Python code with GIL
    /// </summary>
    public T ExecuteWithGIL<T>(Func<T> action)
    {
        if (!_initialized)
            throw new InvalidOperationException("Python bridge not initialized");
        
        using (Py.GIL())
        {
            return action();
        }
    }
    
    /// <summary>
    /// Execute Python code with GIL (void)
    /// </summary>
    public void ExecuteWithGIL(Action action)
    {
        if (!_initialized)
            throw new InvalidOperationException("Python bridge not initialized");
        
        using (Py.GIL())
        {
            action();
        }
    }
    
    /// <summary>
    /// Check if Python bridge is initialized
    /// </summary>
    public bool IsInitialized => _initialized;
    
    /// <summary>
    /// Import Python module using direct Python.NET API
    /// </summary>
    private dynamic Import(string moduleName)
    {
        return Py.Import(moduleName);
    }
    
    /// <summary>
    /// Dispose the Python bridge
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                if (_initialized)
                {
                    using (Py.GIL())
                    {
                        _eventClient = null;
                        _conductorClient = null;
                    }
                    
                    // Shutdown Python runtime using direct Python.NET API
                    if (PythonEngine.IsInitialized)
                    {
                        PythonEngine.Shutdown();
                        _logger.Log("Python runtime shutdown");
                    }
                }
                
                _logger.Log("Python bridge shutdown successfully");
            }
            catch (Exception ex)
            {
                _logger.Log($"Error during Python bridge shutdown: {ex.Message}");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
} 