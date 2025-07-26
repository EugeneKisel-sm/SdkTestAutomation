using Conductor.Api;
using Conductor.Client;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Sdk;
using SdkTestAutomation.Sdk.Adapters;
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
    
    public override SdkResponse<GetEventResponse> AddEvent(AddEventRequest request)
    {
        try
        {
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            _eventApi.AddEventHandler(eventHandler);
            return SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> GetEvent(GetEventRequest request)
    {
        try
        {
            var events = _eventApi.GetEventHandlers();
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromCSharp(firstEvent));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> GetEventByName(GetEventByNameRequest request)
    {
        try
        {
            var events = _eventApi.GetEventHandlersForEvent(request.Event, request.ActiveOnly);
            var firstEvent = events.FirstOrDefault();
            return SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromCSharp(firstEvent));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> UpdateEvent(UpdateEventRequest request)
    {
        try
        {
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            _eventApi.UpdateEventHandler(eventHandler);
            return SdkResponse<GetEventResponse>.CreateSuccess(CreateResponseFromRequest(request));
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    public override SdkResponse<GetEventResponse> DeleteEvent(DeleteEventRequest request)
    {
        try
        {
            _eventApi.RemoveEventHandlerStatus(request.Name);
            return SdkResponse<GetEventResponse>.CreateSuccess(new GetEventResponse());
        }
        catch (Exception ex)
        {
            return SdkResponse<GetEventResponse>.CreateError(ex.Message);
        }
    }
    
    protected override string GetSdkVersion() => "1.1.3";
    
    protected override bool IsInitialized() => _eventApi != null;
    
    protected override void DisposeEngine()
    {
        _eventApi = null;
    }
} 