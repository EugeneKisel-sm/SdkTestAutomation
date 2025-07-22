using System.Text.Json;
using SdkTestAutomation.Api.Conductor.EventResource.Request;

namespace SdkTestAutomation.CSharp.Commands;

public static class EventCommands
{
    public static string BuildAddEventCommand(AddEventRequest request)
    {
        var actions = request.Actions?.Select(a => 
            $"--action {a.Action} " +
            (a.StartWorkflow != null ? $"--start-workflow {JsonSerializer.Serialize(a.StartWorkflow)} " : "") +
            (a.CompleteTask != null ? $"--complete-task {JsonSerializer.Serialize(a.CompleteTask)} " : "") +
            (a.FailTask != null ? $"--fail-task {JsonSerializer.Serialize(a.FailTask)} " : "") +
            (a.TerminateWorkflow != null ? $"--terminate-workflow {JsonSerializer.Serialize(a.TerminateWorkflow)} " : "") +
            (a.UpdateWorkflow != null ? $"--update-workflow {JsonSerializer.Serialize(a.UpdateWorkflow)} " : "") +
            $"--expand-inline-json {a.ExpandInlineJson.ToString().ToLower()}")
        .ToList() ?? new List<string>();
        
        return $"event add " +
               $"--name \"{request.Name}\" " +
               $"--event \"{request.Event}\" " +
               (request.Condition != null ? $"--condition \"{request.Condition}\" " : "") +
               $"--active {request.Active.ToString().ToLower()} " +
               $"--evaluator-type {request.EvaluatorType.ToString().ToLower()} " +
               string.Join(" ", actions);
    }
    
    public static string BuildGetEventCommand(GetEventRequest request)
    {
        return "event list";
    }
    
    public static string BuildGetEventByNameCommand(GetEventByNameRequest request, string eventName)
    {
        var command = $"event get --name \"{eventName}\"";
        if (request.ActiveOnly.HasValue)
        {
            command += $" --active-only {request.ActiveOnly.Value.ToString().ToLower()}";
        }
        return command;
    }
    
    public static string BuildDeleteEventCommand(DeleteEventRequest request, string eventName)
    {
        return $"event delete --name \"{eventName}\"";
    }
    
    public static string BuildUpdateEventCommand(AddEventRequest request)
    {
        return BuildAddEventCommand(request).Replace("event add", "event update");
    }
} 