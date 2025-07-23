using System.CommandLine;
using System.Text.Json;
using Conductor.Api;
using Conductor.Client;
using Conductor.Client.Models;
using SdkTestAutomation.Sdk.Models;
using SdkTestAutomation.CSharp.Operations;

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
        var result = ExecuteOperation(operation, parameters, resource);
        Console.Write(JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
    catch (Exception ex)
    {
        var error = SdkResponse.CreateError(500, ex.Message);
        Console.Write(JsonSerializer.Serialize(error, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        Environment.Exit(1);
    }
}, operationOption, parametersOption, resourceOption);

await rootCommand.InvokeAsync(args);

static SdkResponse ExecuteOperation(string operation, string parameters, string resource)
{
    var paramsDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(parameters) ?? new Dictionary<string, JsonElement>();
    
    return resource switch
    {
        "event" => EventOperations.Execute(operation, paramsDict),
        "workflow" => WorkflowOperations.Execute(operation, paramsDict),
        _ => throw new ArgumentException($"Unknown resource: {resource}")
    };
}
