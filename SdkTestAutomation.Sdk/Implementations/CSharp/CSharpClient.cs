using Conductor.Api;
using Conductor.Client;
using SdkTestAutomation.Sdk.Core.Interfaces;

namespace SdkTestAutomation.Sdk.Implementations.CSharp;

public class CSharpClient : ISdkClient
{
    public WorkflowResourceApi WorkflowApi { get; set; }
    public EventResourceApi EventApi { get; set; }
    private bool _initialized;
    
    public bool IsInitialized => _initialized && WorkflowApi != null && EventApi != null;
    
    public void Initialize(string serverUrl)
    {
        var configuration = new Configuration { BasePath = serverUrl };
        WorkflowApi = new WorkflowResourceApi(configuration);
        EventApi = new EventResourceApi(configuration);
        _initialized = true;
    }

    public void Dispose()
    {
        WorkflowApi = null;
        EventApi = null;
        _initialized = false;
    }
} 