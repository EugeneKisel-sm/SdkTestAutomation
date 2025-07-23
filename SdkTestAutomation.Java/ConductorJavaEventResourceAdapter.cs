using System.Text.Json;
using SdkTestAutomation.Common.Helpers;
using SdkTestAutomation.Common.Interfaces;
using SdkTestAutomation.Common.Models;
using SdkTestAutomation.Java.JavaBridge;

namespace SdkTestAutomation.Java;

/// <summary>
/// Simplified Java SDK adapter for event resource operations using IKVM.NET
/// </summary>
public class ConductorJavaEventResourceAdapter : BaseEventResourceAdapter
{
    private JavaEngine? _javaEngine;
    
    public override string SdkType => "java";
    
    public override async Task<bool> InitializeAsync(AdapterConfiguration config)
    {
        try
        {
            _config = config;
            LogOperation("Initializing Java SDK adapter", config.ServerUrl);
            
            // Initialize Java bridge using IKVM
            _javaEngine = new JavaEngine();
            _javaEngine.Initialize(config);
            
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
            if (_javaEngine == null) return false;
            
            // Try to get events to check if the API is accessible
            await Task.Run(() => _javaEngine!.GetEventHandlers("", false));
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
            
            var eventHandler = CreateEventHandler(request);
            await Task.Run(() => _javaEngine!.RegisterEventHandler(eventHandler));
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
            
            var events = await Task.Run(() => _javaEngine!.GetEventHandlers("", false));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapJavaCollection(events)
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
            LogOperation("Getting events by name", request.Event);
            
            var events = await Task.Run(() => _javaEngine!.GetEventHandlers(request.Event, request.ActiveOnly ?? false));
            
            var data = new GetEventResponse
            {
                Events = EventInfoMapper.MapJavaCollection(events)
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
            
            var eventHandler = CreateEventHandler(request);
            await Task.Run(() => _javaEngine!.UpdateEventHandler(eventHandler));
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
            
            await Task.Run(() => _javaEngine!.UnregisterEventHandler(request.Name));
            return SdkResponseBuilder.CreateEmptyResponse();
        }
        catch (Exception ex)
        {
            LogError("deleting event", ex);
            return SdkResponseBuilder.CreateErrorResponse(ex.Message);
        }
    }
    
    /// <summary>
    /// Create a Java EventHandler object using IKVM
    /// </summary>
    private dynamic CreateEventHandler(AddEventRequest request)
    {
        try
        {
            // Create EventHandler using IKVM
            var eventHandler = _javaEngine!.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler");
            eventHandler.setName(request.Name);
            eventHandler.setEvent(request.Event);
            eventHandler.setActive(request.Active);
            
            // Set actions if any
            if (request.Actions?.Any() == true)
            {
                var actions = new List<dynamic>();
                foreach (var action in request.Actions)
                {
                    var javaAction = _javaEngine!.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler$Action");
                    javaAction.setAction(action.Action);
                    
                    if (action.StartWorkflow != null)
                    {
                        var startWorkflow = _javaEngine!.CreateInstance("com.netflix.conductor.common.metadata.workflow.StartWorkflowRequest");
                        startWorkflow.setName(action.StartWorkflow.Name);
                        startWorkflow.setVersion(action.StartWorkflow.Version);
                        startWorkflow.setInput(JsonSerializer.Serialize(action.StartWorkflow.Input));
                        javaAction.setStartWorkflow(startWorkflow);
                    }
                    
                    actions.Add(javaAction);
                }
                eventHandler.setActions(actions);
            }
            
            return eventHandler;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Java EventHandler: {ex.Message}", ex);
        }
    }
    
    /// <summary>
    /// Create a Java EventHandler object using IKVM
    /// </summary>
    private dynamic CreateEventHandler(UpdateEventRequest request)
    {
        try
        {
            // Create EventHandler using IKVM
            var eventHandler = _javaEngine!.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler");
            eventHandler.setName(request.Name);
            eventHandler.setEvent(request.Event);
            eventHandler.setActive(request.Active);
            
            // Set actions if any
            if (request.Actions?.Any() == true)
            {
                var actions = new List<dynamic>();
                foreach (var action in request.Actions)
                {
                    var javaAction = _javaEngine!.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler$Action");
                    javaAction.setAction(action.Action);
                    
                    if (action.StartWorkflow != null)
                    {
                        var startWorkflow = _javaEngine!.CreateInstance("com.netflix.conductor.common.metadata.workflow.StartWorkflowRequest");
                        startWorkflow.setName(action.StartWorkflow.Name);
                        startWorkflow.setVersion(action.StartWorkflow.Version);
                        startWorkflow.setInput(JsonSerializer.Serialize(action.StartWorkflow.Input));
                        javaAction.setStartWorkflow(startWorkflow);
                    }
                    
                    actions.Add(javaAction);
                }
                eventHandler.setActions(actions);
            }
            
            return eventHandler;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create Java EventHandler: {ex.Message}", ex);
        }
    }
    
    protected override string GetSdkVersion() => "3.15.0";
    
    protected override bool IsInitialized() => _javaEngine != null;
    
    public override void Dispose()
    {
        try
        {
            _javaEngine?.Dispose();
        }
        catch (Exception ex)
        {
            LogError("disposing Java adapter", ex);
        }
    }
} 