using SdkTestAutomation.Sdk.Interfaces;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;

namespace  SdkTestAutomation.Sdk.Helpers;

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
        return await CreateAdapterAsync<IEventResourceAdapter>(sdkType, "event resource");
    }
    
    /// <summary>
    /// Create a workflow resource adapter for the specified SDK type
    /// </summary>
    public static async Task<IWorkflowResourceAdapter> CreateWorkflowResourceAdapterAsync(string sdkType)
    {
        return await CreateAdapterAsync<IWorkflowResourceAdapter>(sdkType, "workflow resource");
    }
    
    /// <summary>
    /// Generic method to create adapters
    /// </summary>
    private static async Task<T> CreateAdapterAsync<T>(string sdkType, string adapterType) where T : class
    {
        _logger.Log($"Creating {adapterType} adapter for SDK: {sdkType}");
        
        var adapter = sdkType.ToLowerInvariant() switch
        {
            "csharp" => CreateAdapterInstance<T>($"SdkTestAutomation.CSharp.ConductorCSharp{GetAdapterClassName<T>()}, SdkTestAutomation.CSharp"),
            "java" => CreateAdapterInstance<T>($"SdkTestAutomation.Java.ConductorJava{GetAdapterClassName<T>()}, SdkTestAutomation.Java"),
            "python" => CreateAdapterInstance<T>($"SdkTestAutomation.Python.ConductorPython{GetAdapterClassName<T>()}, SdkTestAutomation.Python"),
            _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
        };
        
        var config = CreateConfiguration();
        var initialized = await ((ISdkAdapter)adapter).InitializeAsync(config);
        
        if (!initialized)
        {
            throw new Exception($"Failed to initialize {sdkType} {adapterType} adapter");
        }
        
        return adapter;
    }
    
    /// <summary>
    /// Get adapter class name based on interface type
    /// </summary>
    private static string GetAdapterClassName<T>()
    {
        return typeof(T).Name.Replace("I", "").Replace("Adapter", "") + "Adapter";
    }
    
    /// <summary>
    /// Create an adapter instance using reflection
    /// </summary>
    private static T CreateAdapterInstance<T>(string typeName) where T : class
    {
        var type = Type.GetType(typeName) ?? 
                   AppDomain.CurrentDomain.GetAssemblies()
                       .Select(asm => asm.GetType(typeName))
                       .FirstOrDefault(t => t != null);
        
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
    /// Create configuration from environment variables
    /// </summary>
    private static AdapterConfiguration CreateConfiguration()
    {
        return new AdapterConfiguration
        {
            ServerUrl = TestConfig.ApiUrl,
            PythonHome = TestConfig.GetEnvironmentVariable("PYTHON_HOME"),
            PythonPath = TestConfig.GetEnvironmentVariable("PYTHONPATH")
        };
    }
} 