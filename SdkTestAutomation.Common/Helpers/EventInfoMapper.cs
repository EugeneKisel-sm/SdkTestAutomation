using System.Text.Json;
using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Common.Helpers;

/// <summary>
/// Shared helper class for mapping event handlers from different SDKs to common EventInfo model
/// </summary>
public static class EventInfoMapper
{
    /// <summary>
    /// Map C# SDK EventHandler to EventInfo
    /// </summary>
    public static EventInfo MapFromCSharp(dynamic eventHandler)
    {
        var actions = new List<EventAction>();
        
        // Handle actions manually to avoid lambda with dynamic
        if (eventHandler.Actions != null)
        {
            foreach (var a in eventHandler.Actions)
            {
                var action = new EventAction
                {
                    Action = a.Action?.ToString() ?? ""
                };
                
                if (a.StartWorkflow != null)
                {
                    action.StartWorkflow = new StartWorkflow
                    {
                        Name = a.StartWorkflow.Name?.ToString() ?? "",
                        Version = a.StartWorkflow.Version ?? 1,
                        Input = a.StartWorkflow.Input as Dictionary<string, object> ?? new Dictionary<string, object>()
                    };
                }
                
                actions.Add(action);
            }
        }
        
        return new EventInfo
        {
            Name = eventHandler.Name?.ToString() ?? "",
            Event = eventHandler.Event?.ToString() ?? "",
            Active = eventHandler.Active ?? false,
            Actions = actions,
            Condition = eventHandler.Condition?.ToString(),
            EvaluatorType = eventHandler.EvaluatorType?.ToString()
        };
    }
    
    /// <summary>
    /// Map Java SDK EventHandler to EventInfo
    /// </summary>
    public static EventInfo MapFromJava(dynamic javaEvent)
    {
        var eventInfo = new EventInfo
        {
            Name = javaEvent.getName()?.ToString() ?? "",
            Event = javaEvent.getEvent()?.ToString() ?? "",
            Active = javaEvent.isActive() ?? false,
            Condition = javaEvent.getCondition()?.ToString(),
            EvaluatorType = javaEvent.getEvaluatorType()?.ToString(),
            Actions = new List<EventAction>()
        };
        
        // Map actions if present
        var javaActions = javaEvent.getActions();
        if (javaActions != null)
        {
            var actionList = javaActions as IEnumerable<dynamic> ?? new List<dynamic>();
            foreach (var javaAction in actionList)
            {
                var action = new EventAction
                {
                    Action = javaAction.getAction()?.ToString() ?? ""
                };
                
                var startWorkflow = javaAction.getStartWorkflow();
                if (startWorkflow != null)
                {
                    action.StartWorkflow = new StartWorkflow
                    {
                        Name = startWorkflow.getName()?.ToString() ?? "",
                        Version = startWorkflow.getVersion() ?? 1,
                        Input = JsonSerializer.Deserialize<Dictionary<string, object>>(startWorkflow.getInput()?.ToString() ?? "{}") ?? new Dictionary<string, object>()
                    };
                }
                
                eventInfo.Actions.Add(action);
            }
        }
        
        return eventInfo;
    }
    
    /// <summary>
    /// Map Python SDK EventHandler to EventInfo
    /// </summary>
    public static EventInfo MapFromPython(dynamic pythonEvent)
    {
        var eventInfo = new EventInfo
        {
            Name = pythonEvent.name?.ToString() ?? "",
            Event = pythonEvent.event_name?.ToString() ?? "",
            Active = pythonEvent.active ?? false,
            Condition = pythonEvent.condition?.ToString(),
            EvaluatorType = pythonEvent.evaluator_type?.ToString(),
            Actions = new List<EventAction>()
        };
        
        // Map actions if present
        var pythonActions = pythonEvent.actions;
        if (pythonActions != null)
        {
            var actionList = pythonActions as IEnumerable<dynamic> ?? new List<dynamic>();
            foreach (var pythonAction in actionList)
            {
                var action = new EventAction
                {
                    Action = pythonAction.action?.ToString() ?? ""
                };
                
                var startWorkflow = pythonAction.start_workflow;
                if (startWorkflow != null)
                {
                    action.StartWorkflow = new StartWorkflow
                    {
                        Name = startWorkflow.name?.ToString() ?? "",
                        Version = startWorkflow.version ?? 1,
                        Input = JsonSerializer.Deserialize<Dictionary<string, object>>(startWorkflow.input?.ToString() ?? "{}") ?? new Dictionary<string, object>()
                    };
                }
                
                eventInfo.Actions.Add(action);
            }
        }
        
        return eventInfo;
    }
    
    /// <summary>
    /// Map a collection of C# event handlers
    /// </summary>
    public static List<EventInfo> MapCSharpCollection(dynamic events)
    {
        var result = new List<EventInfo>();
        
        try
        {
            // Handle both List and array types
            var eventList = events as IEnumerable<dynamic> ?? new List<dynamic>();
            
            foreach (var eventHandler in eventList)
            {
                result.Add(MapFromCSharp(eventHandler));
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the entire operation
            Console.WriteLine($"Error mapping C# event handlers: {ex.Message}");
        }
        
        return result;
    }
    
    /// <summary>
    /// Map a collection of Java event handlers
    /// </summary>
    public static List<EventInfo> MapJavaCollection(dynamic events)
    {
        var result = new List<EventInfo>();
        
        try
        {
            // Handle both List and array types
            var eventList = events as IEnumerable<dynamic> ?? new List<dynamic>();
            
            foreach (var eventHandler in eventList)
            {
                result.Add(MapFromJava(eventHandler));
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the entire operation
            Console.WriteLine($"Error mapping Java event handlers: {ex.Message}");
        }
        
        return result;
    }
    
    /// <summary>
    /// Map a collection of Python event handlers
    /// </summary>
    public static List<EventInfo> MapPythonCollection(dynamic events)
    {
        var result = new List<EventInfo>();
        
        try
        {
            // Handle both List and array types
            var eventList = events as IEnumerable<dynamic> ?? new List<dynamic>();
            
            foreach (var eventHandler in eventList)
            {
                result.Add(MapFromPython(eventHandler));
            }
        }
        catch (Exception ex)
        {
            // Log error but don't fail the entire operation
            Console.WriteLine($"Error mapping Python event handlers: {ex.Message}");
        }
        
        return result;
    }
} 