using System.CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
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
        Console.Write(JsonConvert.SerializeObject(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
    }
    catch (Exception ex)
    {
        var error = SdkResponse.CreateError(500, ex.Message);
        Console.Write(JsonConvert.SerializeObject(error, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        Environment.Exit(1);
    }
}, operationOption, parametersOption, resourceOption);

await rootCommand.InvokeAsync(args);

static SdkResponse ExecuteOperation(string operation, string parameters, string resource)
{
    var paramsDict = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(parameters) ?? new Dictionary<string, JToken>();
    
    return resource switch
    {
        "event" => EventOperations.Execute(operation, paramsDict),
        "workflow" => WorkflowOperations.Execute(operation, paramsDict),
        _ => throw new ArgumentException($"Unknown resource: {resource}")
    };
}
