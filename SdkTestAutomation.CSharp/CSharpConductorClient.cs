using Conductor.Api;
using Conductor.Client;
using SdkTestAutomation.Sdk.Adapters;

namespace SdkTestAutomation.CSharp;

public class CSharpConductorClient : ConductorClient
{
    private WorkflowResourceApi _workflowApi;
    private EventResourceApi _eventApi;
    
    public CSharpConductorClient(string serverUrl) : base(serverUrl)
    {
        var configuration = new Configuration { BasePath = serverUrl };
        _workflowApi = new WorkflowResourceApi(configuration);
        _eventApi = new EventResourceApi(configuration);
    }
    
    public override bool IsInitialized => _workflowApi != null && _eventApi != null;
    
    public WorkflowResourceApi WorkflowApi
    {
        get
        {
            ThrowIfNotInitialized("Workflow API");
            return _workflowApi;
        }
    }
    
    public EventResourceApi EventApi
    {
        get
        {
            ThrowIfNotInitialized("Event API");
            return _eventApi;
        }
    }
    
    protected override void OnDispose()
    {
        _workflowApi = null;
        _eventApi = null;
    }
} 