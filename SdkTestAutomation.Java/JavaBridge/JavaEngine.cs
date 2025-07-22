using MASES.JCOBridge;
using MASES.JCOBridge.C2JBridge;
using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Java.JavaBridge;

/// <summary>
/// Simplified Java bridge engine using MASES.JCOBridge
/// </summary>
public class JavaEngine : SetupJVMWrapper, IDisposable
{
    private bool _disposed = false;
    
    /// <summary>
    /// Initialize the Java bridge
    /// </summary>
    public void Initialize(AdapterConfiguration config)
    {
        // Set Java home if provided
        if (!string.IsNullOrEmpty(config.JavaHome))
        {
            Environment.SetEnvironmentVariable("JAVA_HOME", config.JavaHome);
        }
        
        // The JVM is automatically initialized when needed
    }
    
    /// <summary>
    /// Create a Java object instance using dynamic access
    /// </summary>
    public dynamic CreateInstance(string className, params object[] args)
    {
        return DynJVM.GetClass(className).NewInstance(args);
    }
    
    /// <summary>
    /// Call a static method on a Java class using dynamic access
    /// </summary>
    public dynamic CallStaticMethod(string className, string methodName, params object[] args)
    {
        return DynJVM.GetClass(className).Invoke(methodName, args);
    }
    
    /// <summary>
    /// Get a Java class for direct access
    /// </summary>
    public dynamic GetClass(string className)
    {
        return DynJVM.GetClass(className);
    }
    
    /// <summary>
    /// Dispose the Java bridge
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            // The JVM will be automatically cleaned up
            _disposed = true;
        }
    }
} 