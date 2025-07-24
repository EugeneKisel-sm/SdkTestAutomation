using System.Text.Json;
using Python.Runtime;
using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Python.Helpers;

/// <summary>
/// Helper class for building Python EventHandler objects
/// </summary>
public static class PythonEventHandlerBuilder
{
    /// <summary>
    /// Create a Python EventHandler from C# request
    /// </summary>
    public static dynamic CreateEventHandler(dynamic request)
    {
        return CreateEventHandlerInternal(request.Name, request.Event, request.Active, request.Actions);
    }
    
    /// <summary>
    /// Internal method to create EventHandler with common logic
    /// </summary>
    private static dynamic CreateEventHandlerInternal(string name, string eventName, bool active, List<EventAction> actions)
    {
        using (Py.GIL())
        {
            dynamic eventHandlerModule = Py.Import("conductor.common.metadata.events.event_handler");
            dynamic EventHandler = eventHandlerModule.GetAttr("EventHandler");
            dynamic eventHandler = EventHandler.Invoke();
            
            eventHandler.name = name;
            eventHandler.event_name = eventName;
            eventHandler.active = active;
            
            if (actions?.Any() == true)
            {
                eventHandler.actions = CreateActions(actions);
            }
            
            return eventHandler;
        }
    }
    
    /// <summary>
    /// Create Python actions from C# actions
    /// </summary>
    private static List<dynamic> CreateActions(IEnumerable<EventAction> actions)
    {
        var pythonActions = new List<dynamic>();
        dynamic eventHandlerModule = Py.Import("conductor.common.metadata.events.event_handler");
        
        foreach (var action in actions)
        {
            dynamic ActionClass = eventHandlerModule.GetAttr("Action");
            dynamic pythonAction = ActionClass.Invoke();
            pythonAction.action = action.Action;
            
            if (action.StartWorkflow != null)
            {
                pythonAction.start_workflow = CreateStartWorkflow(action.StartWorkflow);
            }
            
            pythonActions.Add(pythonAction);
        }
        
        return pythonActions;
    }
    
    /// <summary>
    /// Create Python StartWorkflowRequest from C# StartWorkflow
    /// </summary>
    private static dynamic CreateStartWorkflow(StartWorkflow startWorkflow)
    {
        using (Py.GIL())
        {
            dynamic startWorkflowModule = Py.Import("conductor.common.metadata.workflow.start_workflow_request");
            dynamic StartWorkflowRequest = startWorkflowModule.GetAttr("StartWorkflowRequest");
            dynamic pythonStartWorkflow = StartWorkflowRequest.Invoke();
            
            pythonStartWorkflow.name = startWorkflow.Name;
            pythonStartWorkflow.version = startWorkflow.Version;
            pythonStartWorkflow.input = JsonSerializer.Serialize(startWorkflow.Input);
            
            return pythonStartWorkflow;
        }
    }
} 