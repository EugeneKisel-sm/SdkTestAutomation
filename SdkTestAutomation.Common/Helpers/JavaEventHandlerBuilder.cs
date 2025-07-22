using System.Text.Json;
using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Common.Helpers;

/// <summary>
/// Helper class for building Java EventHandler objects
/// </summary>
public static class JavaEventHandlerBuilder
{
    /// <summary>
    /// Create a Java EventHandler from C# request
    /// </summary>
    public static dynamic CreateEventHandler(dynamic javaEngine, AddEventRequest request)
    {
        var eventHandler = javaEngine.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler");
        eventHandler.setName(request.Name);
        eventHandler.setEvent(request.Event);
        eventHandler.setActive(request.Active);
        
        // Set actions if any
        if (request.Actions?.Any() == true)
        {
            var actions = new List<dynamic>();
            foreach (var action in request.Actions)
            {
                var javaAction = javaEngine.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler$Action");
                javaAction.setAction(action.Action);
                
                if (action.StartWorkflow != null)
                {
                    var startWorkflow = javaEngine.CreateInstance("com.netflix.conductor.common.metadata.workflow.StartWorkflowRequest");
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
    
    /// <summary>
    /// Create a Java EventHandler from C# update request
    /// </summary>
    public static dynamic CreateEventHandler(dynamic javaEngine, UpdateEventRequest request)
    {
        var eventHandler = javaEngine.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler");
        eventHandler.setName(request.Name);
        eventHandler.setEvent(request.Event);
        eventHandler.setActive(request.Active);
        
        // Set actions if any
        if (request.Actions?.Any() == true)
        {
            var actions = new List<dynamic>();
            foreach (var action in request.Actions)
            {
                var javaAction = javaEngine.CreateInstance("com.netflix.conductor.common.metadata.events.EventHandler$Action");
                javaAction.setAction(action.Action);
                
                if (action.StartWorkflow != null)
                {
                    var startWorkflow = javaEngine.CreateInstance("com.netflix.conductor.common.metadata.workflow.StartWorkflowRequest");
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
} 