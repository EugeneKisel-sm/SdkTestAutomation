using System.CommandLine;
using System.Text.Json;
using Conductor.Api;
using Conductor.Client;
using Conductor.Client.Models;
using SdkTestAutomation.Common.Models;

var rootCommand = new RootCommand("C# Conductor SDK Test Wrapper");

var operationOption = new Option<string>("--operation", "SDK operation");
var parametersOption = new Option<string>("--parameters", "JSON parameters");
var resourceOption = new Option<string>("--resource", "Resource type");

rootCommand.AddOption(operationOption);
rootCommand.AddOption(parametersOption);
rootCommand.AddOption(resourceOption);

rootCommand.SetHandler((operation, parameters, resource) =>
{
    try
    {
        var serverUrl = Environment.GetEnvironmentVariable("CONDUCTOR_SERVER_URL") ?? "http://localhost:8080/api";
        var config = new Configuration { BasePath = serverUrl };
        
        var eventApi = new EventResourceApi(config);
        var workflowApi = new WorkflowResourceApi(config);
        
        var result = ExecuteSdkOperation(operation, parameters, resource, eventApi, workflowApi);
        Console.Write(JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
    catch (Exception ex)
    {
        var error = new SdkResponse { Success = false, ErrorMessage = ex.Message, StatusCode = 500 };
        Console.Write(JsonSerializer.Serialize(error, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        Environment.Exit(1);
    }
}, operationOption, parametersOption, resourceOption);

await rootCommand.InvokeAsync(args);

static SdkResponse ExecuteSdkOperation(string operation, string parameters, string resource, 
    EventResourceApi eventApi, WorkflowResourceApi workflowApi)
{
    var paramsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(parameters) ?? new Dictionary<string, JsonElement>();
    
    return operation switch
    {
        "add-event" => AddEvent(paramsDict, eventApi),
        "get-event" => GetEvent(paramsDict, eventApi),
        "get-event-by-name" => GetEventByName(paramsDict, eventApi),
        "update-event" => UpdateEvent(paramsDict, eventApi),
        "delete-event" => DeleteEvent(paramsDict, eventApi),
        "get-workflow" => GetWorkflow(paramsDict, workflowApi),
        _ => throw new ArgumentException($"Unknown operation: {operation}")
    };
}

static SdkResponse AddEvent(Dictionary<string, JsonElement> parameters, EventResourceApi eventApi)
{
    try
    {
        var name = parameters.GetValueOrDefault("name").GetString() ?? "";
        var eventName = parameters.ContainsKey("event") ? parameters["event"].GetString() ?? "" : "";
        var active = parameters.ContainsKey("active") && parameters["active"].GetBoolean();
        
        // For now, use empty actions list to test basic functionality
        var actions = new List<System.Action>();
        
        var eventHandler = new Conductor.Client.Models.EventHandler(
            actions: actions,
            active: active,
            condition: null,
            evaluatorType: null,
            _event: eventName,
            name: name
        );
        
        eventApi.AddEventHandler(eventHandler);
        return new SdkResponse
        {
            StatusCode = 200,
            Success = true
        };
    }
    catch (ApiException ex)
    {
        return new SdkResponse
        {
            StatusCode = ex.ErrorCode,
            Success = false,
            ErrorMessage = ex.Message
        };
    }
}

static SdkResponse GetEvent(Dictionary<string, JsonElement> parameters, EventResourceApi eventApi)
{
    try
    {
        var events = eventApi.GetEventHandlers();
        return new SdkResponse
        {
            StatusCode = 200,
            Success = true,
            Data = events,
            Content = JsonSerializer.Serialize(events)
        };
    }
    catch (ApiException ex)
    {
        return new SdkResponse
        {
            StatusCode = ex.ErrorCode,
            Success = false,
            ErrorMessage = ex.Message
        };
    }
}

static SdkResponse GetEventByName(Dictionary<string, JsonElement> parameters, EventResourceApi eventApi)
{
    try
    {
        var eventName = parameters.GetValueOrDefault("event").GetString() ?? "";
        var activeOnly = parameters.ContainsKey("activeOnly") ? parameters["activeOnly"].GetBoolean() : (bool?)null;
        var events = eventApi.GetEventHandlersForEvent(eventName, activeOnly);
        return new SdkResponse
        {
            StatusCode = 200,
            Success = true,
            Data = events,
            Content = JsonSerializer.Serialize(events)
        };
    }
    catch (ApiException ex)
    {
        return new SdkResponse
        {
            StatusCode = ex.ErrorCode,
            Success = false,
            ErrorMessage = ex.Message
        };
    }
}

static SdkResponse UpdateEvent(Dictionary<string, JsonElement> parameters, EventResourceApi eventApi)
{
    try
    {
        var name = parameters.GetValueOrDefault("name").GetString() ?? "";
        var eventName = parameters.ContainsKey("event") ? parameters["event"].GetString() ?? "" : "";
        var active = parameters.ContainsKey("active") && parameters["active"].GetBoolean();
        
        // For now, use empty actions list to test basic functionality
        var actions = new List<System.Action>();
        
        var eventHandler = new Conductor.Client.Models.EventHandler(
            actions: actions,
            active: active,
            condition: null,
            evaluatorType: null,
            _event: eventName,
            name: name
        );
        
        eventApi.UpdateEventHandler(eventHandler);
        return new SdkResponse
        {
            StatusCode = 200,
            Success = true
        };
    }
    catch (ApiException ex)
    {
        return new SdkResponse
        {
            StatusCode = ex.ErrorCode,
            Success = false,
            ErrorMessage = ex.Message
        };
    }
}

static SdkResponse DeleteEvent(Dictionary<string, JsonElement> parameters, EventResourceApi eventApi)
{
    try
    {
        var eventName = parameters.GetValueOrDefault("name").GetString() ?? "";
        eventApi.RemoveEventHandlerStatus(eventName);
        return new SdkResponse
        {
            StatusCode = 200,
            Success = true
        };
    }
    catch (ApiException ex)
    {
        return new SdkResponse
        {
            StatusCode = ex.ErrorCode,
            Success = false,
            ErrorMessage = ex.Message
        };
    }
}

static SdkResponse GetWorkflow(Dictionary<string, JsonElement> parameters, WorkflowResourceApi workflowApi)
{
    try
    {
        var workflowId = parameters.GetValueOrDefault("workflowId").GetString() ?? "";
        var workflow = workflowApi.GetExecutionStatus(workflowId);
        return new SdkResponse
        {
            StatusCode = 200,
            Success = true,
            Data = workflow,
            Content = JsonSerializer.Serialize(workflow)
        };
    }
    catch (ApiException ex)
    {
        return new SdkResponse
        {
            StatusCode = ex.ErrorCode,
            Success = false,
            ErrorMessage = ex.Message
        };
    }
}
