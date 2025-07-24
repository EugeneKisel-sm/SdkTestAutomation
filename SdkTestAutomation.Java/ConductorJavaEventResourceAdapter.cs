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
    private JavaEngine _javaEngine;
    private const string EVENT_HANDLER_CLASS = "com.netflix.conductor.common.metadata.events.EventHandler";
    private const string ACTION_CLASS = "com.netflix.conductor.common.metadata.events.EventHandler$Action";
    private const string START_WORKFLOW_CLASS = "com.netflix.conductor.common.metadata.workflow.StartWorkflowRequest";
    
    public override string SdkType => "java";
    
    protected override async Task InitializeEngineAsync(AdapterConfiguration config)
    {
        _javaEngine = new JavaEngine();
        _javaEngine.Initialize(config);
        await Task.CompletedTask;
    }
    
    protected override void PerformHealthCheck()
    {
        _javaEngine.GetEventHandlers("", false);
    }
    
    public override Task<SdkResponse<GetEventResponse>> AddEventAsync(AddEventRequest request)
    {
        return ExecuteOperationAsync($"Adding event: {request.Name}", () =>
        {
            var eventHandler = CreateEventHandler(request);
            _javaEngine.RegisterEventHandler(eventHandler);
            return Task.FromResult(SdkResponseBuilder.CreateFromRequest(request));
        });
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventAsync(GetEventRequest request)
    {
        return ExecuteOperationAsync("Getting all events", () =>
        {
            var events = _javaEngine.GetEventHandlers("", false);
            return Task.FromResult(CreateSuccessResponseWithEvents(events, "MapJavaCollection"));
        });
    }
    
    public override Task<SdkResponse<GetEventResponse>> GetEventByNameAsync(GetEventByNameRequest request)
    {
        return ExecuteOperationAsync($"Getting events by name: {request.Event}", () =>
        {
            var events = _javaEngine.GetEventHandlers(request.Event, request.ActiveOnly ?? false);
            return Task.FromResult(CreateSuccessResponseWithEvents(events, "MapJavaCollection"));
        });
    }
    
    public override Task<SdkResponse<GetEventResponse>> UpdateEventAsync(UpdateEventRequest request)
    {
        return ExecuteOperationAsync($"Updating event: {request.Name}", () =>
        {
            var eventHandler = CreateEventHandler(request);
            _javaEngine.UpdateEventHandler(eventHandler);
            return Task.FromResult(SdkResponseBuilder.CreateFromRequest(request));
        });
    }
    
    public override Task<SdkResponse<GetEventResponse>> DeleteEventAsync(DeleteEventRequest request)
    {
        return ExecuteOperationAsync($"Deleting event: {request.Name}", () =>
        {
            _javaEngine.UnregisterEventHandler(request.Name);
            return Task.FromResult(SdkResponseBuilder.CreateEmptyResponse());
        });
    }
    
    /// <summary>
    /// Create a Java EventHandler object using IKVM with optimized object creation
    /// </summary>
    private dynamic CreateEventHandler(dynamic request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
            
        if (string.IsNullOrEmpty(request.Name))
            throw new ArgumentException("Event handler name is required", nameof(request));
            
        try
        {
            // Create EventHandler using IKVM - cache the type lookup
            var eventHandler = _javaEngine.CreateInstance(EVENT_HANDLER_CLASS);
            eventHandler.setName(request.Name);
            eventHandler.setEvent(request.Event);
            eventHandler.setActive(request.Active);
            
            // Set actions if any - optimize collection handling
            if (request.Actions?.Any() == true)
            {
                var actions = new List<dynamic>();
                foreach (var action in request.Actions)
                {
                    var javaAction = _javaEngine.CreateInstance(ACTION_CLASS);
                    javaAction.setAction(action.Action);
                    
                    if (action.StartWorkflow != null)
                    {
                        var startWorkflow = _javaEngine.CreateInstance(START_WORKFLOW_CLASS);
                        startWorkflow.setName(action.StartWorkflow.Name);
                        startWorkflow.setVersion(action.StartWorkflow.Version);
                        
                        // Only serialize if input exists
                        if (action.StartWorkflow.Input != null)
                        {
                            startWorkflow.setInput(JsonSerializer.Serialize(action.StartWorkflow.Input));
                        }
                        
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
            throw new InvalidOperationException(
                $"Failed to create Java EventHandler '{request.Name}': {ex.Message}", 
                ex);
        }
    }
    
    protected override string GetSdkVersion() => "3.15.0";
    
    protected override bool IsInitialized() => _javaEngine != null;
    
    protected override void DisposeEngine()
    {
        _javaEngine?.Dispose();
    }
} 