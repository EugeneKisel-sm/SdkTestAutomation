using Conductor.Api;
using Conductor.Client;
using Conductor.Client.Models;
using SdkTestAutomation.Common.Helpers;
using SdkTestAutomation.Common.Interfaces;
using SdkTestAutomation.Common.Models;
using Task = System.Threading.Tasks.Task;
using EventHandler = Conductor.Client.Models.EventHandler;

namespace SdkTestAutomation.CSharp;

/// <summary>
/// C# SDK adapter for event resource operations
/// </summary>
public class ConductorCSharpEventResourceAdapter : BaseEventResourceAdapter
{
    private EventResourceApi _eventApi;
    
    public override string SdkType => "csharp";
    
    protected override async Task InitializeEngineAsync(AdapterConfiguration config)
    {
        var configuration = new Configuration { BasePath = config.ServerUrl };
        _eventApi = new EventResourceApi(configuration);
        await Task.CompletedTask;
    }
    
    protected override void PerformHealthCheck()
    {
        _eventApi.GetEventHandlers();
    }
    
    public override Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        return ExecuteOperationAsync($"Adding event: {request.Name}", () =>
        {
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            _eventApi.AddEventHandler(eventHandler);
            return Task.FromResult(SdkResponseBuilder.CreateFromRequest(request));
        }, ex => GetApiExceptionStatusCode(ex));
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        return ExecuteOperationAsync("Getting all events", () =>
        {
            var events = _eventApi.GetEventHandlers();
            return Task.FromResult(CreateSuccessResponseWithEvents(events, "MapCSharpCollection"));
        }, ex => GetApiExceptionStatusCode(ex));
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        return ExecuteOperationAsync($"Getting events by name: {request.Event}", () =>
        {
            var events = _eventApi.GetEventHandlersForEvent(request.Event, request.ActiveOnly);
            return Task.FromResult(CreateSuccessResponseWithEvents(events, "MapCSharpCollection"));
        }, ex => GetApiExceptionStatusCode(ex));
    }
    
    public override Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        return ExecuteOperationAsync($"Updating event: {request.Name}", () =>
        {
            var eventHandler = new EventHandler
            {
                Name = request.Name,
                Active = request.Active
            };
            
            _eventApi.UpdateEventHandler(eventHandler);
            return Task.FromResult(SdkResponseBuilder.CreateFromRequest(request));
        }, ex => GetApiExceptionStatusCode(ex));
    }
    
    public override Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        return ExecuteOperationAsync($"Deleting event: {request.Name}", () =>
        {
            _eventApi.RemoveEventHandlerStatus(request.Name);
            return Task.FromResult(SdkResponseBuilder.CreateEmptyResponse());
        }, ex => GetApiExceptionStatusCode(ex));
    }
    
    protected override string GetSdkVersion() => "1.1.3";
    
    protected override bool IsInitialized() => _eventApi != null;
    
    protected override int GetApiExceptionStatusCode(Exception ex, int defaultCode = 500)
    {
        return ex is ApiException apiEx ? apiEx.ErrorCode : defaultCode;
    }
    
    protected override void DisposeEngine()
    {
        _eventApi = null;
    }
} 