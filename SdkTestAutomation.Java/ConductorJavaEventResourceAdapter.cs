using System.Text.Json;
using SdkTestAutomation.Common.Helpers;
using SdkTestAutomation.Common.Interfaces;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Java.JavaBridge;

namespace SdkTestAutomation.Java;

/// <summary>
/// Java SDK adapter for event resource operations using MASES.JCOBridge
/// </summary>
public class ConductorJavaEventResourceAdapter : BaseEventResourceAdapter
{
    private dynamic? _eventClient;
    private JavaEngine? _javaEngine;
    
    public override string SdkType => "java";
    
    public override async Task<bool> InitializeAsync(AdapterConfiguration config)
    {
        try
        {
            _config = config;
            LogOperation("Initializing Java SDK adapter", config.ServerUrl);
            
            // Initialize Java bridge
            _javaEngine = new JavaEngine();
            _javaEngine.Initialize(config);
            
            // Create Conductor client using JCOBridge
            var conductorClient = _javaEngine.CreateInstance("com.netflix.conductor.client.http.ConductorClient", config.ServerUrl);
            _eventClient = _javaEngine.CreateInstance("com.netflix.conductor.client.http.EventClient", conductorClient);
            
            LogOperation("Java SDK adapter initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            LogError("initializing Java SDK adapter", ex);
            return false;
        }
    }
    
    public override async Task<bool> IsHealthyAsync()
    {
        try
        {
            if (_eventClient == null) return false;
            
            // Try to get events to check if the API is accessible
            await Task.Run(() => _eventClient!.getEventHandlers("", false));
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
            var eventHandler = JavaEventHandlerBuilder.CreateEventHandler(_javaEngine!, request);
            await Task.Run(() => _eventClient!.registerEventHandler(eventHandler));
            return SdkResponseBuilder.CreateFromRequest(request);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        return await ExecuteEventOperation("Getting all events", null, request.RequestId, async () =>
        {
            var events = await Task.Run(() => _eventClient!.getEventHandlers("", false));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapJavaCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data, request.RequestId);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        return await ExecuteEventOperation("Getting events by name", request.Event, request.RequestId, async () =>
        {
            var events = await Task.Run(() => _eventClient!.getEventHandlers(request.Event, request.ActiveOnly ?? false));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapJavaCollection(events)
            };
            
            return SdkResponseBuilder.CreateSuccessResponse(data, request.RequestId);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        return await ExecuteEventOperation("Updating event", request.Name, request.RequestId, async () =>
        {
            var eventHandler = JavaEventHandlerBuilder.CreateEventHandler(_javaEngine!, request);
            await Task.Run(() => _eventClient!.updateEventHandler(eventHandler));
            return SdkResponseBuilder.CreateFromRequest(request);
        });
    }
    
    public override async Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        return await ExecuteEventOperation("Deleting event", request.Name, request.RequestId, async () =>
        {
            await Task.Run(() => _eventClient!.unregisterEventHandler(request.Name));
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
    
    protected override bool IsInitialized() => _eventClient != null;
    
    public override void Dispose()
    {
        try
        {
            _eventClient = null;
            _javaEngine?.Dispose();
        }
        catch (Exception ex)
        {
            LogError("disposing Java adapter", ex);
        }
    }
} 