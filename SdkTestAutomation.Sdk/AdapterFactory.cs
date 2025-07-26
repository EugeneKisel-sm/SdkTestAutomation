using SdkTestAutomation.Sdk.Interfaces;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Utils;
using SdkTestAutomation.Utils.Logging;

namespace  SdkTestAutomation.Sdk;

public class AdapterFactory(ILogger logger, string sdkType)
{
    private readonly ILogger _logger = logger;
    private readonly string _sdkType = sdkType;
    
    public IEventResourceAdapter CreateEventResourceAdapter()
    {
        return CreateAdapter<IEventResourceAdapter>(_sdkType, "event resource");
    }
    
    public IWorkflowResourceAdapter CreateWorkflowResourceAdapter()
    {
        return CreateAdapter<IWorkflowResourceAdapter>(_sdkType, "workflow resource");
    }
    
    private T CreateAdapter<T>(string sdkType, string adapterType) where T : class
    {
        _logger.Log($"Creating {adapterType} adapter for SDK: {sdkType}");
        
        try
        {
            var adapter = sdkType.ToLowerInvariant() switch
            {
                "csharp" => CreateAdapterInstance<T>($"SdkTestAutomation.CSharp.ConductorCSharp{GetAdapterClassName<T>()}, SdkTestAutomation.CSharp"),
                "java" => CreateAdapterInstance<T>($"SdkTestAutomation.Java.ConductorJava{GetAdapterClassName<T>()}, SdkTestAutomation.Java"),
                "python" => CreateAdapterInstance<T>($"SdkTestAutomation.Python.ConductorPython{GetAdapterClassName<T>()}, SdkTestAutomation.Python"),
                _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}. Supported types are: csharp, java, python")
            };
            
            var config = CreateConfiguration();
            var initialized = ((ISdkAdapter)adapter).Initialize(config);
            
            if (!initialized)
            {
                throw new InvalidOperationException($"Failed to initialize {sdkType} {adapterType} adapter. Check server URL and SDK configuration.");
            }
            
            return adapter;
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"Failed to create {sdkType} {adapterType} adapter: {ex.Message}", ex);
        }
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
            throw new InvalidOperationException($"Could not find adapter type: {typeName}. Make sure the SDK assembly is referenced and loaded.");
        }
        
        try
        {
            var instance = Activator.CreateInstance(type);
            if (instance is not T adapter)
            {
                throw new InvalidOperationException($"Type {typeName} does not implement {typeof(T).Name}. Check SDK implementation.");
            }
            
            return adapter;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw new InvalidOperationException($"Failed to create instance of {typeName}: {ex.Message}", ex);
        }
    }
    
    private static AdapterConfiguration CreateConfiguration()
    {
        var serverUrl = TestConfig.ApiUrl;
        if (string.IsNullOrEmpty(serverUrl))
        {
            throw new InvalidOperationException("Server URL is not configured. Set CONDUCTOR_SERVER_URL environment variable.");
        }
        
        return new AdapterConfiguration
        {
            ServerUrl = serverUrl,
            PythonHome = TestConfig.GetEnvironmentVariable("PYTHON_HOME"),
            PythonPath = TestConfig.GetEnvironmentVariable("PYTHONPATH")
        };
    }
} 