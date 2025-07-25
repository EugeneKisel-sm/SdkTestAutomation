using Conductor.Api;
using Conductor.Client;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Sdk.Helpers;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using EventHandler = Conductor.Client.Models.EventHandler;

namespace SdkTestAutomation.CSharp;

/// <summary>
/// C# SDK adapter for event resource operations
/// </summary>
public class ConductorCSharpEventResourceAdapter : BaseEventResourceAdapter
{
    private EventResourceApi _eventApi;
    
    public override string SdkType => "csharp";
    
    protected override Task InitializeEngineAsync()
    {
        var configuration = new Configuration { BasePath = Config.ServerUrl };
        _eventApi = new EventResourceApi(configuration);
        return Task.CompletedTask;
    }
    
    protected override void PerformHealthCheck()
    {
        _eventApi.GetEventHandlers();
    }
    
    public override Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        try
        {
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            _eventApi.AddEventHandler(eventHandler);
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        try
        {
            var events = _eventApi.GetEventHandlers();
            var firstEvent = events.FirstOrDefault();
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromCSharp(firstEvent)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        try
        {
            var events = _eventApi.GetEventHandlersForEvent(request.Event, request.ActiveOnly);
            var firstEvent = events.FirstOrDefault();
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromCSharp(firstEvent)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    public override Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        try
        {
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            _eventApi.UpdateEventHandler(eventHandler);
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request)));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    public override Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        try
        {
            _eventApi.RemoveEventHandlerStatus(request.Name);
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(new GetEventResponse()));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    protected override string GetSdkVersion() => "1.1.3";
    
    protected override bool IsInitialized() => _eventApi != null;
    
    protected override void DisposeEngine()
    {
        _eventApi = null;
    }
} 