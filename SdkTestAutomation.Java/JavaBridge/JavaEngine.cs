using MASES.JCOBridge;
using MASES.JCOBridge.C2JBridge;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Java.JavaBridge;

/// <summary>
/// Java bridge engine using MASES.JCOBridge
/// </summary>
public class JavaEngine : SetupJVMWrapper, IDisposable
{
    private static readonly ILogger _logger = new ConsoleLogger(null);
    private bool _disposed = false;
    
    /// <summary>
    /// Initialize the Java bridge
    /// </summary>
    public void Initialize(AdapterConfiguration config)
    {
        try
        {
            _logger.Log("Initializing Java bridge...");
            
            // Set Java home if provided
            if (!string.IsNullOrEmpty(config.JavaHome))
            {
                Environment.SetEnvironmentVariable("JAVA_HOME", config.JavaHome);
                _logger.Log($"Set JAVA_HOME to: {config.JavaHome}");
            }
            
            // Set JVM options if provided
            if (config.JavaOptions?.Any() == true)
            {
                var options = string.Join(" ", config.JavaOptions);
                _logger.Log($"JVM options: {options}");
            }
            
            // Initialize JVM using JCOBridge
            // The JVM is automatically initialized when needed
            
            _logger.Log("Java bridge initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.Log($"Failed to initialize Java bridge: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Create a Java object instance using dynamic access
    /// </summary>
    public dynamic CreateInstance(string className, params object[] args)
    {
        try
        {
            // Use dynamic access as shown in the documentation
            return DynJVM.GetClass(className).NewInstance(args);
        }
        catch (Exception ex)
        {
            _logger.Log($"Failed to create Java instance {className}: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Call a static method on a Java class using dynamic access
    /// </summary>
    public dynamic CallStaticMethod(string className, string methodName, params object[] args)
    {
        try
        {
            // Use dynamic access as shown in the documentation
            return DynJVM.GetClass(className).Invoke(methodName, args);
        }
        catch (Exception ex)
        {
            _logger.Log($"Failed to call static method {className}.{methodName}: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Get a Java class for direct access
    /// </summary>
    public dynamic GetClass(string className)
    {
        try
        {
            return DynJVM.GetClass(className);
        }
        catch (Exception ex)
        {
            _logger.Log($"Failed to get Java class {className}: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Dispose the Java bridge
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            try
            {
                // Shutdown JVM
                // The JVM will be automatically cleaned up
                _logger.Log("Java bridge shutdown successfully");
            }
            catch (Exception ex)
            {
                _logger.Log($"Error during Java bridge shutdown: {ex.Message}");
            }
            finally
            {
                _disposed = true;
            }
        }
    }
} 