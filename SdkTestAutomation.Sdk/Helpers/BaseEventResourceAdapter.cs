using SdkTestAutomation.Sdk.Interfaces;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;

namespace SdkTestAutomation.Sdk.Helpers;

public abstract class BaseEventResourceAdapter : IEventResourceAdapter
{
    protected AdapterConfiguration Config { get; private set; }
    
    public abstract string SdkType { get; }
    
    public virtual async Task<bool> InitializeAsync(AdapterConfiguration config)
    {
        try
        {
            Config = config;
            await InitializeEngineAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public virtual bool IsHealthy()
    {
        try
        {
            PerformHealthCheck();
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public virtual AdapterInfo GetAdapterInfo()
    {
        return new AdapterInfo
        {
            SdkType = SdkType,
            Version = GetSdkVersion(),
            IsInitialized = IsInitialized(),
            InitializedAt = DateTime.UtcNow
        };
    }
    
    public abstract SdkResponse<GetEventResponse> AddEvent(AddEventRequest request);
    public abstract SdkResponse<GetEventResponse> GetEvent(GetEventRequest request);
    public abstract SdkResponse<GetEventResponse> GetEventByName(GetEventByNameRequest request);
    public abstract SdkResponse<GetEventResponse> UpdateEvent(UpdateEventRequest request);
    public abstract SdkResponse<GetEventResponse> DeleteEvent(DeleteEventRequest request);
    
    public virtual void Dispose()
    {
        DisposeEngine();
    }
    
    protected abstract Task InitializeEngineAsync();
    protected abstract void PerformHealthCheck();
    protected abstract bool IsInitialized();
    protected abstract void DisposeEngine();
    protected virtual string GetSdkVersion() => "1.0.0";
    
    /// <summary>
    /// Helper method to create GetEventResponse from request
    /// </summary>
    protected static GetEventResponse CreateResponseFromRequest(dynamic request)
    {
        return new GetEventResponse
        {
            Name = request.Name,
            Event = request.Event,
            Active = request.Active,
            Actions = request.Actions,
            Condition = request.Condition,
            EvaluatorType = request.EvaluatorType
        };
    }
} 