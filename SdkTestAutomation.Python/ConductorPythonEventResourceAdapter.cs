using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.Sdk.Helpers;
using SdkTestAutomation.Api.Conductor.EventResource.Request;
using SdkTestAutomation.Api.Conductor.EventResource.Response;
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
    
    protected override Task InitializeEngineAsync()
    {
        _pythonEngine = new PythonBridgeEngine();
        _pythonEngine.Initialize(Config);
        
        _eventClient = _pythonEngine.ExecuteWithGIL(() => _pythonEngine.GetEventClient());
        return Task.CompletedTask;
    }
    
    protected override void PerformHealthCheck()
    {
        _pythonEngine.ExecuteWithGIL(() => _eventClient.get_event_handlers("", false));
    }
    
    public override Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        try
        {
            _pythonEngine.ExecuteWithGIL(() => 
            {
                var eventHandler = PythonEventHandlerBuilder.CreateEventHandler(request);
                _eventClient.register_event_handler(eventHandler);
            });
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
            var events = _pythonEngine.ExecuteWithGIL(() => _eventClient.get_event_handlers("", false));
            var firstEvent = events.FirstOrDefault();
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromPython(firstEvent)));
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
            var events = _pythonEngine.ExecuteWithGIL(() => 
                _eventClient.get_event_handlers(request.Event, request.ActiveOnly ?? false));
            var firstEvent = events.FirstOrDefault();
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(EventInfoMapper.MapFromPython(firstEvent)));
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
            _pythonEngine.ExecuteWithGIL(() => 
            {
                var eventHandler = PythonEventHandlerBuilder.CreateEventHandler(request);
                _eventClient.update_event_handler(eventHandler);
            });
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
            _pythonEngine.ExecuteWithGIL(() => _eventClient.unregister_event_handler(request.Name));
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateSuccess(new GetEventResponse()));
        }
        catch (Exception ex)
        {
            return Task.FromResult(SdkResponse<GetEventResponse>.CreateError(ex.Message));
        }
    }
    
    protected override string GetSdkVersion() => "4.0.0";
    
    protected override bool IsInitialized() => _pythonEngine != null && _eventClient != null;
    
    protected override void DisposeEngine()
    {
        _eventClient = null;
        _pythonEngine = null;
    }
} 