using Newtonsoft.Json.Linq;
using Conductor.Api;
using Conductor.Client;
using Conductor.Client.Models;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.CSharp.Extensions;

namespace SdkTestAutomation.CSharp.Operations;

public static class EventOperations
{
    public static SdkResponse Execute(string operation, Dictionary<string, JToken> parameters)
    {
        return OperationUtils.ExecuteWithErrorHandling(() =>
        {
            var config = OperationUtils.CreateSdkConfiguration();
            var eventApi = new EventResourceApi(config);
            
            return operation switch
            {
                "add-event" => AddEvent(parameters, eventApi),
                "get-event" => GetEvent(eventApi),
                "get-event-by-name" => GetEventByName(parameters, eventApi),
                "update-event" => UpdateEvent(parameters, eventApi),
                "delete-event" => DeleteEvent(parameters, eventApi),
                _ => throw new ArgumentException($"Unknown event operation: {operation}")
            };
        });
    }
    
    private static SdkResponse AddEvent(Dictionary<string, JToken> parameters, EventResourceApi eventApi)
    {
        var eventHandler = CreateEventHandler(parameters);
        eventApi.AddEventHandler(eventHandler);
        return SdkResponse.CreateSuccess();
    }
    
    private static SdkResponse GetEvent(EventResourceApi eventApi)
    {
        var events = eventApi.GetEventHandlers();
        return SdkResponse.CreateSuccess(events);
    }
    
    private static SdkResponse GetEventByName(Dictionary<string, JToken> parameters, EventResourceApi eventApi)
    {
        var eventName = parameters.GetString("event");
        var activeOnly = parameters.GetBoolNullable("activeOnly");
        
        var events = eventApi.GetEventHandlersForEvent(eventName, activeOnly);
        return SdkResponse.CreateSuccess(events);
    }
    
    private static SdkResponse UpdateEvent(Dictionary<string, JToken> parameters, EventResourceApi eventApi)
    {
        var eventHandler = CreateEventHandler(parameters);
        eventApi.UpdateEventHandler(eventHandler);
        return SdkResponse.CreateSuccess();
    }
    
    private static SdkResponse DeleteEvent(Dictionary<string, JToken> parameters, EventResourceApi eventApi)
    {
        var eventName = parameters.GetString("name");
        eventApi.RemoveEventHandlerStatus(eventName);
        return SdkResponse.CreateSuccess();
    }
    
    private static Conductor.Client.Models.EventHandler CreateEventHandler(Dictionary<string, JToken> parameters)
    {
        return new Conductor.Client.Models.EventHandler(
            actions: new List<System.Action>(),
            active: parameters.GetBool("active"),
            condition: null,
            evaluatorType: null,
            _event: parameters.GetString("event"),
            name: parameters.GetString("name")
        );
    }
} 