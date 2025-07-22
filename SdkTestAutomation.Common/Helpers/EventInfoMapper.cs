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
        return new EventInfo
        {
            Name = eventHandler.Name?.ToString() ?? "",
            Event = eventHandler.Event?.ToString() ?? "",
            Active = eventHandler.Active ?? false,
            Actions = MapActionsFromCSharp(eventHandler.Actions),
            Condition = eventHandler.Condition?.ToString(),
            EvaluatorType = eventHandler.EvaluatorType?.ToString()
        };
    }
    
    /// <summary>
    /// Map Java SDK EventHandler to EventInfo
    /// </summary>
    public static EventInfo MapFromJava(dynamic javaEvent)
    {
        return new EventInfo
        {
            Name = javaEvent.getName()?.ToString() ?? "",
            Event = javaEvent.getEvent()?.ToString() ?? "",
            Active = javaEvent.isActive() ?? false,
            Actions = MapActionsFromJava(javaEvent.getActions()),
            Condition = javaEvent.getCondition()?.ToString(),
            EvaluatorType = javaEvent.getEvaluatorType()?.ToString()
        };
    }
    
    /// <summary>
    /// Map Python SDK EventHandler to EventInfo
    /// </summary>
    public static EventInfo MapFromPython(dynamic pythonEvent)
    {
        return new EventInfo
        {
            Name = pythonEvent.name?.ToString() ?? "",
            Event = pythonEvent.event_name?.ToString() ?? "",
            Active = pythonEvent.active ?? false,
            Actions = MapActionsFromPython(pythonEvent.actions),
            Condition = pythonEvent.condition?.ToString(),
            EvaluatorType = pythonEvent.evaluator_type?.ToString()
        };
    }
    
    /// <summary>
    /// Map a collection of C# event handlers
    /// </summary>
    public static List<EventInfo> MapCSharpCollection(dynamic events)
    {
        var result = new List<EventInfo>();
        
        try
        {
            var eventList = events as IEnumerable<dynamic> ?? new List<dynamic>();
            foreach (var eventHandler in eventList)
            {
                result.Add(MapFromCSharp(eventHandler));
            }
        }
        catch (Exception ex)
        {
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
            var eventList = events as IEnumerable<dynamic> ?? new List<dynamic>();
            foreach (var eventHandler in eventList)
            {
                result.Add(MapFromJava(eventHandler));
            }
        }
        catch (Exception ex)
        {
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
            var eventList = events as IEnumerable<dynamic> ?? new List<dynamic>();
            foreach (var eventHandler in eventList)
            {
                result.Add(MapFromPython(eventHandler));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mapping Python event handlers: {ex.Message}");
        }
        
        return result;
    }
    
    /// <summary>
    /// Map C# actions to EventAction list
    /// </summary>
    private static List<EventAction> MapActionsFromCSharp(dynamic? actions)
    {
        var result = new List<EventAction>();
        
        if (actions == null) return result;
        
        foreach (var a in actions)
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
            
            result.Add(action);
        }
        
        return result;
    }
    
    /// <summary>
    /// Map Java actions to EventAction list
    /// </summary>
    private static List<EventAction> MapActionsFromJava(dynamic? javaActions)
    {
        var result = new List<EventAction>();
        
        if (javaActions == null) return result;
        
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
            
            result.Add(action);
        }
        
        return result;
    }
    
    /// <summary>
    /// Map Python actions to EventAction list
    /// </summary>
    private static List<EventAction> MapActionsFromPython(dynamic? pythonActions)
    {
        var result = new List<EventAction>();
        
        if (pythonActions == null) return result;
        
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
            
            result.Add(action);
        }
        
        return result;
    }
} 