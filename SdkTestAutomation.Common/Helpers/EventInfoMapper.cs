using System.Text.Json;
using SdkTestAutomation.Common.Models;

namespace SdkTestAutomation.Common.Helpers;

/// <summary>
/// Simplified helper for mapping event handlers from different SDKs to common EventInfo model
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
            Name = GetPropertyValue(eventHandler, "Name"),
            Event = GetPropertyValue(eventHandler, "Event"),
            Active = GetPropertyValue(eventHandler, "Active") ?? false,
            Actions = MapActionsFromCSharp(eventHandler.Actions),
            Condition = GetPropertyValue(eventHandler, "Condition"),
            EvaluatorType = GetPropertyValue(eventHandler, "EvaluatorType")
        };
    }
    
    /// <summary>
    /// Map Java SDK EventHandler to EventInfo
    /// </summary>
    public static EventInfo MapFromJava(dynamic javaEvent)
    {
        return new EventInfo
        {
            Name = GetMethodValue(javaEvent, "getName"),
            Event = GetMethodValue(javaEvent, "getEvent"),
            Active = GetMethodValue(javaEvent, "isActive") ?? false,
            Actions = MapActionsFromJava(javaEvent.getActions()),
            Condition = GetMethodValue(javaEvent, "getCondition"),
            EvaluatorType = GetMethodValue(javaEvent, "getEvaluatorType")
        };
    }
    
    /// <summary>
    /// Map Python SDK EventHandler to EventInfo
    /// </summary>
    public static EventInfo MapFromPython(dynamic pythonEvent)
    {
        return new EventInfo
        {
            Name = GetPropertyValue(pythonEvent, "name"),
            Event = GetPropertyValue(pythonEvent, "event_name"),
            Active = GetPropertyValue(pythonEvent, "active") ?? false,
            Actions = MapActionsFromPython(pythonEvent.actions),
            Condition = GetPropertyValue(pythonEvent, "condition"),
            EvaluatorType = GetPropertyValue(pythonEvent, "evaluator_type")
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
    private static List<EventAction> MapActionsFromCSharp(dynamic actions)
    {
        var result = new List<EventAction>();
        
        if (actions == null) return result;
        
        foreach (var a in actions)
        {
            var action = new EventAction
            {
                Action = GetPropertyValue(a, "Action")
            };
            
            if (a.StartWorkflow != null)
            {
                action.StartWorkflow = new StartWorkflow
                {
                    Name = GetPropertyValue(a.StartWorkflow, "Name"),
                    Version = GetPropertyValue(a.StartWorkflow, "Version") ?? 1,
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
    private static List<EventAction> MapActionsFromJava(dynamic javaActions)
    {
        var result = new List<EventAction>();
        
        if (javaActions == null) return result;
        
        var actionList = javaActions as IEnumerable<dynamic> ?? new List<dynamic>();
        foreach (var javaAction in actionList)
        {
            var action = new EventAction
            {
                Action = GetMethodValue(javaAction, "getAction")
            };
            
            var startWorkflow = javaAction.getStartWorkflow();
            if (startWorkflow != null)
            {
                action.StartWorkflow = new StartWorkflow
                {
                    Name = GetMethodValue(startWorkflow, "getName"),
                    Version = GetMethodValue(startWorkflow, "getVersion") ?? 1,
                    Input = JsonSerializer.Deserialize<Dictionary<string, object>>(GetMethodValue(startWorkflow, "getInput") ?? "{}") ?? new Dictionary<string, object>()
                };
            }
            
            result.Add(action);
        }
        
        return result;
    }
    
    /// <summary>
    /// Map Python actions to EventAction list
    /// </summary>
    private static List<EventAction> MapActionsFromPython(dynamic pythonActions)
    {
        var result = new List<EventAction>();
        
        if (pythonActions == null) return result;
        
        var actionList = pythonActions as IEnumerable<dynamic> ?? new List<dynamic>();
        foreach (var pythonAction in actionList)
        {
            var action = new EventAction
            {
                Action = GetPropertyValue(pythonAction, "action")
            };
            
            var startWorkflow = pythonAction.start_workflow;
            if (startWorkflow != null)
            {
                action.StartWorkflow = new StartWorkflow
                {
                    Name = GetPropertyValue(startWorkflow, "name"),
                    Version = GetPropertyValue(startWorkflow, "version") ?? 1,
                    Input = JsonSerializer.Deserialize<Dictionary<string, object>>(GetPropertyValue(startWorkflow, "input") ?? "{}") ?? new Dictionary<string, object>()
                };
            }
            
            result.Add(action);
        }
        
        return result;
    }
    
    /// <summary>
    /// Safely get property value from dynamic object
    /// </summary>
    private static string GetPropertyValue(dynamic obj, string propertyName)
    {
        try
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
                var value = property.GetValue(obj);
                return value?.ToString();
            }
        }
        catch
        {
            // Property doesn't exist or is not accessible
        }
        return string.Empty;
    }
    
    /// <summary>
    /// Safely get method value from dynamic object
    /// </summary>
    private static string GetMethodValue(dynamic obj, string methodName)
    {
        try
        {
            var method = obj.GetType().GetMethod(methodName);
            if (method != null)
            {
                var value = method.Invoke(obj, null);
                return value?.ToString();
            }
        }
        catch
        {
            // Method doesn't exist or is not accessible
        }
        return string.Empty;
    }
} 