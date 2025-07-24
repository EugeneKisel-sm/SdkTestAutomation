# Adding Operations to Java CLI Wrapper

Quick guide for adding new operations to the Java CLI wrapper.

> **See also**: **[C# Adding Operations](../SdkTestAutomation.CSharp/ADDING_OPERATIONS.md)** | **[Python Adding Operations](../SdkTestAutomation.Python/ADDING_OPERATIONS.md)**

## 📁 Architecture

```
SdkTestAutomation.Java/
├── Main.java                     # Main entry point
├── OperationUtils.java           # Common utilities
├── SdkResponse.java              # Response model
├── operations/                   # Operation classes
│   ├── EventOperations.java      # Event operations
│   └── WorkflowOperations.java   # Workflow operations
└── src/main/java/                # Source code
```

## 🎯 Adding Event Operations

### Step 1: Add to EventOperations.java

```java
// In operations/EventOperations.java
return switch (operation) {
    case "add-event" -> addEvent(parameters, eventApi);
    case "get-event" -> getEvent(eventApi);
    case "new-operation" -> newOperation(parameters, eventApi); // ← Add here
    default -> throw new IllegalArgumentException("Unknown event operation: " + operation);
};

// Add the operation method
private static SdkResponse newOperation(Map<String, Object> parameters, EventClient eventApi) throws Exception {
    String param1 = (String) parameters.get("param1");
    Boolean param2 = (Boolean) parameters.get("param2");
    
    Object result = eventApi.yourNewMethod(param1, param2);
    return SdkResponse.createSuccess(result);
}
```

## 🎯 Adding Workflow Operations

### Step 1: Add to WorkflowOperations.java

```java
// In operations/WorkflowOperations.java
return switch (operation) {
    case "get-workflow" -> getWorkflow(parameters, workflowApi);
    case "new-workflow-operation" -> newWorkflowOperation(parameters, workflowApi); // ← Add here
    default -> throw new IllegalArgumentException("Unknown workflow operation: " + operation);
};

private static SdkResponse newWorkflowOperation(Map<String, Object> parameters, WorkflowClient workflowApi) throws Exception {
    String workflowId = (String) parameters.get("workflowId");
    String action = (String) parameters.get("action");
    
    Object result = workflowApi.yourNewMethod(workflowId, action);
    return SdkResponse.createSuccess(result);
}
```

## 🎯 Adding New Resource Types

### Step 1: Create TaskOperations.java

```java
// Create operations/TaskOperations.java
package com.conductor.sdkwrapper.operations;

import com.netflix.conductor.client.http.ConductorClient;
import com.netflix.conductor.client.http.TaskClient;
import com.conductor.sdkwrapper.OperationUtils;
import com.conductor.sdkwrapper.SdkResponse;
import java.util.Map;

public class TaskOperations {
    
    public static SdkResponse execute(String operation, Map<String, Object> parameters) {
        return OperationUtils.executeWithErrorHandling(() -> {
            ConductorClient client = OperationUtils.createSdkConfiguration();
            TaskClient taskApi = new TaskClient(client);
            
            return switch (operation) {
                case "get-task" -> getTask(parameters, taskApi);
                case "update-task" -> updateTask(parameters, taskApi);
                default -> throw new IllegalArgumentException("Unknown task operation: " + operation);
            };
        });
    }
    
    private static SdkResponse getTask(Map<String, Object> parameters, TaskClient taskApi) throws Exception {
        String taskId = (String) parameters.get("taskId");
        Object task = taskApi.getTask(taskId);
        return SdkResponse.createSuccess(task);
    }
}
```

### Step 2: Update Main.java

```java
// In Main.java
return switch (resource) {
    case "event" -> EventOperations.execute(operation, parameters);
    case "workflow" -> WorkflowOperations.execute(operation, parameters);
    case "task" -> TaskOperations.execute(operation, parameters); // ← Add here
    default -> throw new IllegalArgumentException("Unknown resource: " + resource);
};
```

## 🧪 Testing

### Build and Test

```bash
# Build
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Java
mvn clean package

# Test new operation
java -jar target/sdk-wrapper-1.0.0.jar \
  --operation new-operation \
  --parameters '{"param1":"value1","param2":true}' \
  --resource event
```

### Add Test Case

> **Note**: Test cases are the same for all SDKs since they use the .NET test framework. See **[C# Adding Operations](../SdkTestAutomation.CSharp/ADDING_OPERATIONS.md#🧪-testing)** for the complete test example.

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

## 🔍 Example: Pause Workflow

```java
// Add to WorkflowOperations.java
case "pause-workflow" -> pauseWorkflow(parameters, workflowApi),

private static SdkResponse pauseWorkflow(Map<String, Object> parameters, WorkflowClient workflowApi) throws Exception {
    String workflowId = (String) parameters.get("workflowId");
    String reason = (String) parameters.get("reason");
    
    workflowApi.pauseWorkflow(workflowId, reason);
    return SdkResponse.createSuccess();
}
```

Test:
```bash
java -jar target/sdk-wrapper-1.0.0.jar \
  --operation pause-workflow \
  --parameters '{"workflowId":"test-workflow","reason":"Maintenance"}' \
  --resource workflow
``` 