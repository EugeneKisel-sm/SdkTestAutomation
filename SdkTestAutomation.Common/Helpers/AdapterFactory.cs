using SdkTestAutomation.Common.Interfaces;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Utils.Logging;

namespace SdkTestAutomation.Common.Helpers;

/// <summary>
/// Simplified factory for creating SDK adapters
/// </summary>
public static class AdapterFactory
{
    private static readonly ILogger _logger = new ConsoleLogger(null);
    
    /// <summary>
    /// Create an event resource adapter for the specified SDK type
    /// </summary>
    public static async Task<IEventResourceAdapter> CreateEventResourceAdapterAsync(string sdkType)
    {
        _logger.Log($"Creating event resource adapter for SDK type: {sdkType}");
        
        // Use reflection to create adapters to avoid circular dependencies
        IEventResourceAdapter adapter = sdkType.ToLowerInvariant() switch
        {
            "csharp" => CreateAdapterInstance<IEventResourceAdapter>("SdkTestAutomation.CSharp.ConductorCSharpEventResourceAdapter, SdkTestAutomation.CSharp"),
            "java" => CreateAdapterInstance<IEventResourceAdapter>("SdkTestAutomation.Java.ConductorJavaEventResourceAdapter, SdkTestAutomation.Java"),
            "python" => CreateAdapterInstance<IEventResourceAdapter>("SdkTestAutomation.Python.ConductorPythonEventResourceAdapter, SdkTestAutomation.Python"),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
        
        var config = CreateConfiguration();
        var initialized = await adapter.InitializeAsync(config);
        
        if (!initialized)
        {
            throw new Exception($"Failed to initialize {sdkType} adapter");
        }
        
        _logger.Log($"Successfully created {sdkType} event resource adapter");
        return adapter;
    }
    
    /// <summary>
    /// Create a workflow resource adapter for the specified SDK type
    /// </summary>
    public static async Task<IWorkflowResourceAdapter> CreateWorkflowResourceAdapterAsync(string sdkType)
    {
        _logger.Log($"Creating workflow resource adapter for SDK type: {sdkType}");
        
        // Use reflection to create adapters to avoid circular dependencies
        IWorkflowResourceAdapter adapter = sdkType.ToLowerInvariant() switch
        {
            "csharp" => CreateAdapterInstance<IWorkflowResourceAdapter>("SdkTestAutomation.CSharp.ConductorCSharpWorkflowResourceAdapter, SdkTestAutomation.CSharp"),
            "java" => CreateAdapterInstance<IWorkflowResourceAdapter>("SdkTestAutomation.Java.ConductorJavaWorkflowResourceAdapter, SdkTestAutomation.Java"),
            "python" => CreateAdapterInstance<IWorkflowResourceAdapter>("SdkTestAutomation.Python.ConductorPythonWorkflowResourceAdapter, SdkTestAutomation.Python"),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
        
        var config = CreateConfiguration();
        var initialized = await adapter.InitializeAsync(config);
        
        if (!initialized)
        {
            throw new Exception($"Failed to initialize {sdkType} adapter");
        }
        
        _logger.Log($"Successfully created {sdkType} workflow resource adapter");
        return adapter;
    }
    
    /// <summary>
    /// Create an adapter instance using reflection
    /// </summary>
    private static T CreateAdapterInstance<T>(string typeName) where T : class
    {
        var type = FindType(typeName);
        if (type == null)
        {
            throw new InvalidOperationException($"Could not find type: {typeName}");
        }
        
        var instance = Activator.CreateInstance(type);
        if (instance is not T adapter)
        {
            throw new InvalidOperationException($"Type {typeName} does not implement {typeof(T).Name}");
        }
        
        return adapter;
    }
    
    /// <summary>
    /// Find type by searching through all loaded assemblies
    /// </summary>
    private static Type FindType(string typeName)
    {
        // First try the simple approach
        var type = Type.GetType(typeName);
        if (type != null) return type;
        
        // Search through all loaded assemblies
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            type = assembly.GetType(typeName);
            if (type != null) return type;
        }
        
        throw new InvalidOperationException($"Could not find type: {typeName}");
    }
    
    /// <summary>
    /// Create configuration from environment variables
    /// </summary>
    private static AdapterConfiguration CreateConfiguration()
    {
        return new AdapterConfiguration
        {
            ServerUrl = EnvironmentConfig.GetEnvironmentVariable("CONDUCTOR_SERVER_URL", "http://localhost:8080/api"),
            PythonHome = EnvironmentConfig.GetEnvironmentVariable("PYTHON_HOME"),
            PythonPath = EnvironmentConfig.GetEnvironmentVariable("PYTHONPATH"),
            JavaHome = EnvironmentConfig.GetEnvironmentVariable("JAVA_HOME"),
            JavaClassPath = EnvironmentConfig.GetEnvironmentVariable("JAVA_CLASSPATH"),
            EnableLogging = EnvironmentConfig.GetEnvironmentVariable("ENABLE_LOGGING", "true").ToLower() == "true",
            LogLevel = EnvironmentConfig.GetEnvironmentVariable("LOG_LEVEL", "Info")
        };
    }
} 