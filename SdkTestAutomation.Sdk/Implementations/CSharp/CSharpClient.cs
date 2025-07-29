using Conductor.Api;
using Conductor.Client;
using SdkTestAutomation.Sdk.Core.Interfaces;

namespace SdkTestAutomation.Sdk.Implementations.CSharp;

public class CSharpClient : ISdkClient
{
    public WorkflowResourceApi WorkflowApi { get; set; }
    public EventResourceApi EventApi { get; set; }
    public TokenResourceApi TokenApi { get; set; }
    
    private bool _initialized;
    
    public bool IsInitialized => _initialized && WorkflowApi != null && EventApi != null;
    
    public void Initialize(string serverUrl)
    {
        var configuration = new Configuration { BasePath = serverUrl };
        WorkflowApi = new WorkflowResourceApi(configuration);
        EventApi = new EventResourceApi(configuration);
        TokenApi = new TokenResourceApi(configuration);
        _initialized = true;
    }

    public void Dispose()
    {
        WorkflowApi = null;
        EventApi = null;
        TokenApi = null;
        _initialized = false;
    }
} 