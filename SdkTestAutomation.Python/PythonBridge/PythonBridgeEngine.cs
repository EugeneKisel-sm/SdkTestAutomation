using System.Reflection;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Utils.Logging;

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
            
            // Initialize Python runtime
            var pythonEngineType = Type.GetType("Python.Runtime.PythonEngine, pythonnet");
            if (pythonEngineType == null)
            {
                throw new InvalidOperationException("Could not find Python.Runtime.PythonEngine type");
            }
            
            var isInitialized = (bool)pythonEngineType.GetProperty("IsInitialized")!.GetValue(null)!;
            if (!isInitialized)
            {
                pythonEngineType.GetMethod("Initialize")!.Invoke(null, null);
                _logger.Log("Python runtime initialized");
            }
            
            // Import required Python modules
            using (GetGIL())
            {
                // Import conductor SDK
                dynamic conductor = Import("conductor.client.http.conductor_client");
                dynamic eventClient = Import("conductor.client.http.event_client");
                
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
        
        using (GetGIL())
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
        
        using (GetGIL())
        {
            action();
        }
    }
    
    /// <summary>
    /// Check if Python bridge is initialized
    /// </summary>
    public bool IsInitialized => _initialized;
    
    /// <summary>
    /// Get GIL context
    /// </summary>
    private IDisposable GetGIL()
    {
        var pyType = Type.GetType("Python.Runtime.Py, pythonnet");
        return (IDisposable)pyType!.GetMethod("GIL")!.Invoke(null, null)!;
    }
    
    /// <summary>
    /// Import Python module
    /// </summary>
    private dynamic Import(string moduleName)
    {
        var pyType = Type.GetType("Python.Runtime.Py, pythonnet");
        return pyType!.GetMethod("Import")!.Invoke(null, new object[] { moduleName })!;
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
                    using (GetGIL())
                    {
                        _eventClient = null;
                        _conductorClient = null;
                    }
                    
                    // Shutdown Python runtime
                    var pythonEngineType = Type.GetType("Python.Runtime.PythonEngine, pythonnet");
                    if (pythonEngineType != null)
                    {
                        var isInitialized = (bool)pythonEngineType.GetProperty("IsInitialized")!.GetValue(null)!;
                        if (isInitialized)
                        {
                            pythonEngineType.GetMethod("Shutdown")!.Invoke(null, null);
                            _logger.Log("Python runtime shutdown");
                        }
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