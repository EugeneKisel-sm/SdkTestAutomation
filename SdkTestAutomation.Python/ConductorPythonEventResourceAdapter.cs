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
        return await ExecuteEventOperation("Adding event", request.Name, request.RequestId, async () =>
        {
            await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                var eventHandler = PythonEventHandlerBuilder.CreateEventHandler(request);
                eventClient.register_event_handler(eventHandler);
            }));
            return SdkResponseBuilder.CreateFromRequest(request);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        return await ExecuteEventOperation("Getting all events", null, request.RequestId, async () =>
        {
            var events = await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                return eventClient.get_event_handlers("", false);
            }));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapPythonCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data, request.RequestId);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        return await ExecuteEventOperation("Getting event by name", request.Event, request.RequestId, async () =>
        {
            var events = await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                return eventClient.get_event_handlers(request.Event, request.ActiveOnly ?? false);
            }));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapPythonCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data, request.RequestId);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        return await ExecuteEventOperation("Updating event", request.Name, request.RequestId, async () =>
        {
            await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                var eventHandler = PythonEventHandlerBuilder.CreateEventHandler(request);
                eventClient.update_event_handler(eventHandler);
            }));
            return SdkResponseBuilder.CreateFromRequest(request);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        return await ExecuteEventOperation("Deleting event", request.Name, request.RequestId, async () =>
        {
            await Task.Run(() => _pythonEngine!.ExecuteWithGIL(() => 
            {
                var eventClient = _pythonEngine!.GetEventClient();
                eventClient.unregister_event_handler(request.Name);
            }));
            return SdkResponseBuilder.CreateEmptyResponse(request.RequestId);
        });
    }
    
    /// <summary>
    /// Execute event operation with common error handling
    /// </summary>
    private async Task<SdkResponse<GetEventResponse>> ExecuteEventOperation(
        string operation, 
        string? details, 
        string requestId, 
        Func<Task<SdkResponse<GetEventResponse>>> operationFunc)
    {
        try
        {
            ValidateInitialization();
            LogOperation(operation, details);
            
            return await operationFunc();
        }
        catch (Exception ex)
        {
            LogError(operation.ToLower(), ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message, requestId);
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