using Conductor.Api;
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
        WorkflowApi = new WorkflowResourceApi(serverUrl);
        EventApi = new EventResourceApi(serverUrl);
        TokenApi = new TokenResourceApi(serverUrl);
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