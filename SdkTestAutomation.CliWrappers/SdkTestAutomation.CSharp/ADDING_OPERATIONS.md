# Adding Operations to C# CLI Wrapper

Quick guide for adding new operations to the C# CLI wrapper.

> **See also**: **[Java Adding Operations](../SdkTestAutomation.Java/ADDING_OPERATIONS.md)** | **[Python Adding Operations](../SdkTestAutomation.Python/ADDING_OPERATIONS.md)**

## 📁 Structure

```
SdkTestAutomation.CSharp/
├── Program.cs                    # Main entry point
├── OperationUtils.cs             # Common utilities
├── Operations/                   # Operation classes
│   ├── EventOperations.cs        # Event operations
│   └── WorkflowOperations.cs     # Workflow operations
└── Extensions/                   # Parameter extraction helpers
    └── JTokenExtensions.cs
```

## 🎯 Adding Event Operations

### Step 1: Add to EventOperations.cs

```csharp
// In Operations/EventOperations.cs
return operation switch
{
    "add-event" => AddEvent(parameters, eventApi),
    "get-event" => GetEvent(eventApi),
    "new-operation" => NewOperation(parameters, eventApi), // ← Add here
    _ => throw new ArgumentException($"Unknown event operation: {operation}")
};

// Add the operation method
private static SdkResponse NewOperation(Dictionary<string, JToken> parameters, EventResourceApi eventApi)
{
    var param1 = parameters.GetString("param1");
    var param2 = parameters.GetBool("param2");
    
    var result = eventApi.YourNewMethod(param1, param2);
    return SdkResponse.CreateSuccess(result);
}
```

### Step 2: Add Extension Method (if needed)

```csharp
// In Extensions/JTokenExtensions.cs
public static int GetInt(this Dictionary<string, JToken> parameters, string key, int defaultValue = 0)
{
    return parameters.TryGetValue(key, out var element) ? element.GetInt32() : defaultValue;
}
```

## 🎯 Adding Workflow Operations

### Step 1: Add to WorkflowOperations.cs

```csharp
// In Operations/WorkflowOperations.cs
return operation switch
{
    "get-workflow" => GetWorkflow(parameters, workflowApi),
    "new-workflow-operation" => NewWorkflowOperation(parameters, workflowApi), // ← Add here
    _ => throw new ArgumentException($"Unknown workflow operation: {operation}")
};

private static SdkResponse NewWorkflowOperation(Dictionary<string, JToken> parameters, WorkflowResourceApi workflowApi)
{
    var workflowId = parameters.GetString("workflowId");
    var action = parameters.GetString("action");
    
    var result = workflowApi.YourNewMethod(workflowId, action);
    return SdkResponse.CreateSuccess(result);
}
```

## 🎯 Adding New Resource Types

### Step 1: Create TaskOperations.cs

```csharp
// Create Operations/TaskOperations.cs
public static class TaskOperations
{
    public static SdkResponse Execute(string operation, Dictionary<string, JToken> parameters)
    {
        return OperationUtils.ExecuteWithErrorHandling(() =>
        {
            var config = OperationUtils.CreateSdkConfiguration();
            var taskApi = new TaskResourceApi(config);
            
            return operation switch
            {
                "get-task" => GetTask(parameters, taskApi),
                "update-task" => UpdateTask(parameters, taskApi),
                _ => throw new ArgumentException($"Unknown task operation: {operation}")
            };
        });
    }
    
    private static SdkResponse GetTask(Dictionary<string, JToken> parameters, TaskResourceApi taskApi)
    {
        var taskId = parameters.GetString("taskId");
        var task = taskApi.GetTask(taskId);
        return SdkResponse.CreateSuccess(task);
    }
}
```

### Step 2: Update Program.cs

```csharp
// In Program.cs
return resource switch
{
    "event" => EventOperations.Execute(operation, paramsDict),
    "workflow" => WorkflowOperations.Execute(operation, paramsDict),
    "task" => TaskOperations.Execute(operation, paramsDict), // ← Add here
    _ => throw new ArgumentException($"Unknown resource: {resource}")
};
```

## 🧪 Testing

### Build and Test

```bash
# Build
dotnet build SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/SdkTestAutomation.CSharp.csproj

# Test new operation
dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- \
  --operation new-operation \
  --parameters '{"param1":"value1","param2":true}' \
  --resource event
```

### Add Test Case

> **Note**: Test cases are the same for all SDKs since they use the .NET test framework.

```csharp
// In SdkTestAutomation.Tests/Conductor/EventResource/SdkIntegrationTests.cs
[Fact]
public async Task SdkIntegration_NewOperation_ValidatesAgainstApi()
{
    var parameters = new Dictionary<string, object>
    {
        ["param1"] = "test_value",
        ["param2"] = true
    };

    var sdkResponse = await ExecuteSdkCallAsync<object>("new-operation", parameters, "event");
    var apiResponse = EventResourceApi.YourNewMethod("test_value", true);

    Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
    Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse));
}
```

## 📋 Best Practices

- **Error Handling**: Use `OperationUtils.ExecuteWithErrorHandling()` - no try-catch needed
- **Parameters**: Use extension methods from `JTokenExtensions`
- **Responses**: Use `SdkResponse.CreateSuccess()` or `SdkResponse.CreateError()`
- **Naming**: Operations in kebab-case, methods in PascalCase
- **Documentation**: Add XML comments and update README.md

## 🔍 Example: Pause Workflow

```csharp
// Add to WorkflowOperations.cs
case "pause-workflow" => PauseWorkflow(parameters, workflowApi),

private static SdkResponse PauseWorkflow(Dictionary<string, JToken> parameters, WorkflowResourceApi workflowApi)
{
    var workflowId = parameters.GetString("workflowId");
    var reason = parameters.GetString("reason");
    
    workflowApi.PauseWorkflow(workflowId, reason);
    return SdkResponse.CreateSuccess();
}
```

Test:
```bash
dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- \
  --operation pause-workflow \
  --parameters '{"workflowId":"test-workflow","reason":"Maintenance"}' \
  --resource workflow
``` 