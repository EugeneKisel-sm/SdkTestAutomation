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
    
    public virtual Task<bool> IsHealthyAsync()
    {
        try
        {
            PerformHealthCheck();
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
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
    
    public abstract Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request);
    public abstract Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request);
    public abstract Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request);
    public abstract Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request);
    public abstract Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request);
    
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