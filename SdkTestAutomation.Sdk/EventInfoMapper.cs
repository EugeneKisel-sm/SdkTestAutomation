using SdkTestAutomation.Api.Conductor.EventResource.Response;
using SdkTestAutomation.Api.Conductor.EventResource.Models;

namespace SdkTestAutomation.Sdk;

public static class EventInfoMapper
{
    public static GetEventResponse MapFromCSharp(dynamic eventHandler)
    {
        if (eventHandler == null) return new GetEventResponse();
        
        return new GetEventResponse
        {
            Name = eventHandler.Name?.ToString() ?? "",
            Event = eventHandler.Event?.ToString() ?? "",
            Active = eventHandler.Active == true,
            Actions = new List<EventAction>(),
            Condition = eventHandler.Condition?.ToString() ?? "",
            EvaluatorType = eventHandler.EvaluatorType == true
        };
    }
    
    public static GetEventResponse MapFromJava(dynamic javaEvent)
    {
        if (javaEvent == null) return new GetEventResponse();
        
        return new GetEventResponse
        {
            Name = javaEvent.getName()?.ToString() ?? "",
            Event = javaEvent.getEvent()?.ToString() ?? "",
            Active = javaEvent.isActive() == true,
            Actions = new List<EventAction>(),
            Condition = javaEvent.getCondition()?.ToString() ?? "",
            EvaluatorType = javaEvent.getEvaluatorType() == true
        };
    }
    
    public static GetEventResponse MapFromPython(dynamic pythonEvent)
    {
        if (pythonEvent == null) return new GetEventResponse();
        
        return new GetEventResponse
        {
            Name = pythonEvent.name?.ToString() ?? "",
            Event = pythonEvent.event_name?.ToString() ?? "",
            Active = pythonEvent.active == true,
            Actions = new List<EventAction>(),
            Condition = pythonEvent.condition?.ToString() ?? "",
            EvaluatorType = pythonEvent.evaluator_type == true
        };
    }
} 