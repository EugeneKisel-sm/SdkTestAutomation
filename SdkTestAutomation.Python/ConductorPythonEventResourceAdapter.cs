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
    private PythonBridgeEngine? _pythonEngine;
    
    public override string SdkType => "python";
    
    public override async Task<bool> InitializeAsync(AdapterConfiguration config)
    {
        try
        {
            _config = config;
            LogOperation("Initializing Python SDK adapter", config.ServerUrl);
            
            // Initialize Python bridge
            _pythonEngine = new PythonBridgeEngine();
            _pythonEngine.Initialize(config);
            
            LogOperation("Python SDK adapter initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            LogError("initializing Python SDK adapter", ex);
            return false;
        }
    }
    
    public override async Task<bool> IsHealthyAsync()
    {
        try
        {
            if (_pythonEngine == null) return false;
            
            // Try to get events to check if the API is accessible
            await Task.Run(() => _pythonEngine.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                eventClient.get_event_handlers("", false);
            }));
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Adding event", request.Name);
            
            await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                var eventHandler = PythonEventHandlerBuilder.CreateEventHandler(request);
                eventClient.register_event_handler(eventHandler);
            }));
            return SdkResponseBuilder.CreateFromRequest(request);
        }
        catch (Exception ex)
        {
            LogError("adding event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Getting all events");
            
            var events = await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                return eventClient.get_event_handlers("", false);
            }));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapPythonCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data);
        }
        catch (Exception ex)
        {
            LogError("getting events", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Getting event by name", request.Event);
            
            var events = await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                return eventClient.get_event_handlers(request.Event, request.ActiveOnly ?? false);
            }));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapPythonCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data);
        }
        catch (Exception ex)
        {
            LogError("getting events by name", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Updating event", request.Name);
            
            await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                var eventHandler = PythonEventHandlerBuilder.CreateEventHandler(request);
                eventClient.update_event_handler(eventHandler);
            }));
            return SdkResponseBuilder.CreateFromRequest(request);
        }
        catch (Exception ex)
        {
            LogError("updating event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    public override async Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        try
        {
            ValidateInitialization();
            LogOperation("Deleting event", request.Name);
            
            await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                eventClient.unregister_event_handler(request.Name);
            }));
            return SdkResponseBuilder.CreateEmptyResponse();
        }
        catch (Exception ex)
        {
            LogError("deleting event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    protected override string GetSdkVersion() => "4.0.0";
    
    protected override bool IsInitialized() => _pythonEngine != null;
    
    public override void Dispose()
    {
        try
        {
            _pythonEngine?.Dispose();
        }
        catch (Exception ex)
        {
            LogError("disposing Python adapter", ex);
        }
    }
} 