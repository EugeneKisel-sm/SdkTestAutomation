using Python.Runtime;
using SdkTestAutomation.Common.Helpers;
using SdkTestAutomation.Common.Interfaces;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Python.PythonBridge;
using SdkTestAutomation.Python.Helpers;

namespace SdkTestAutomation.Python;

/// <summary>
/// Python SDK adapter for event resource operations using pythonnet
/// </summary>
public class ConductorPythonEventResourceAdapter : BaseEventResourceAdapter
{
    private PythonBridgeEngine _pythonEngine;
    private dynamic _eventClient;
    
    public override string SdkType => "python";
    
    protected override async Task InitializeEngineAsync(AdapterConfiguration config)
    {
        _pythonEngine = new PythonBridgeEngine();
        _pythonEngine.Initialize(config);
        
        // Cache the event client on initialization
        _eventClient = _pythonEngine.ExecuteWithGIL(() => _pythonEngine.GetEventClient());
        await Task.CompletedTask;
    }
    
    protected override void PerformHealthCheck()
    {
        _pythonEngine.ExecuteWithGIL(() => _eventClient.get_event_handlers("", false));
    }
    
    public override Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        return ExecuteOperationAsync($"Adding event: {request.Name}", () =>
        {
            _pythonEngine.ExecuteWithGIL(() => 
            {
                var eventHandler = PythonEventHandlerBuilder.CreateEventHandler(request);
                _eventClient.register_event_handler(eventHandler);
            });
            return Task.FromResult(SdkResponseBuilder.CreateFromRequest(request));
        });
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        return ExecuteOperationAsync("Getting all events", () =>
        {
            var events = _pythonEngine.ExecuteWithGIL(() => _eventClient.get_event_handlers("", false));
            return Task.FromResult(CreateSuccessResponseWithEvents(events, "MapPythonCollection"));
        });
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        return ExecuteOperationAsync($"Getting event by name: {request.Event}", () =>
        {
            var events = _pythonEngine.ExecuteWithGIL(() => 
                _eventClient.get_event_handlers(request.Event, request.ActiveOnly ?? false));
            return Task.FromResult(CreateSuccessResponseWithEvents(events, "MapPythonCollection"));
        });
    }
    
    public override Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        return ExecuteOperationAsync($"Updating event: {request.Name}", () =>
        {
            _pythonEngine.ExecuteWithGIL(() => 
            {
                var eventHandler = PythonEventHandlerBuilder.CreateEventHandler(request);
                _eventClient.update_event_handler(eventHandler);
            });
            return Task.FromResult(SdkResponseBuilder.CreateFromRequest(request));
        });
    }
    
    public override Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        return ExecuteOperationAsync($"Deleting event: {request.Name}", () =>
        {
            _pythonEngine.ExecuteWithGIL(() => _eventClient.unregister_event_handler(request.Name));
            return Task.FromResult(SdkResponseBuilder.CreateEmptyResponse());
        });
    }
    
    protected override string GetSdkVersion() => "4.0.0";
    
    protected override bool IsInitialized() => _pythonEngine != null && _eventClient != null;
    
    protected override void DisposeEngine()
    {
        if (_eventClient != null)
        {
            _pythonEngine.ExecuteWithGIL(() => 
            {
                if (_eventClient is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                _eventClient = null;
            });
        }
        _pythonEngine?.Dispose();
    }
} 