using SdkTestAutomation.Sdk.Interfaces;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;

namespace  SdkTestAutomation.Sdk.Helpers;

public class AdapterFactory(ILogger logger, string sdkType)
{
    private readonly ILogger _logger = logger;
    private readonly string _sdkType = sdkType;
    
    public IEventResourceAdapter CreateEventResourceAdapterAsync()
    {
        return CreateAdapterAsync<IEventResourceAdapter>(_sdkType, "event resource");
    }
    
    public IWorkflowResourceAdapter CreateWorkflowResourceAdapterAsync()
    {
        return CreateAdapterAsync<IWorkflowResourceAdapter>(_sdkType, "workflow resource");
    }
    
    private T CreateAdapterAsync<T>(string sdkType, string adapterType) where T : class
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
        var initialized = ((ISdkAdapter)adapter).InitializeAsync(config).Result;
        
        if (!initialized)
        {
            throw new Exception($"Failed to initialize {sdkType} {adapterType} adapter");
        }
        
        return adapter;
    }
    
    private string GetAdapterClassName<T>()
    {
        return typeof(T).Name.Replace("I", "").Replace("Adapter", "") + "Adapter";
    }
    
    private T CreateAdapterInstance<T>(string typeName) where T : class
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