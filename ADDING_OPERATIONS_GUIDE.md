# Adding Operations Guide

Universal guide for adding new operations to CLI wrappers across all supported languages.

> **Language-specific guides**: **[C#](../SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/ADDING_OPERATIONS.md)** | **[Java](../SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/ADDING_OPERATIONS.md)** | **[Python](../SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/ADDING_OPERATIONS.md)**

## 🎯 Overview

All CLI wrappers follow the same architecture pattern for adding operations:

1. **Add operation to existing resource** (Event, Workflow)
2. **Create new resource type** (if needed)
3. **Update main entry point** to route to new operations
4. **Add parameter extraction helpers** (if needed)

## 📁 Common Structure

```
Wrapper/
├── Main Entry Point          # CLI argument parsing and routing
├── OperationUtils            # Common utilities and error handling
├── Operations/               # Resource-specific operations
│   ├── EventOperations       # Event resource operations
│   ├── WorkflowOperations    # Workflow resource operations
│   └── [NewResource]Operations # New resource operations
└── Extensions/               # Parameter extraction helpers
```

## 🔧 Adding to Existing Resources

### Event Operations
Add new operations to the existing EventOperations class/module:

1. **Add operation case** to the switch/if-else statement
2. **Create operation method** with proper error handling
3. **Use parameter extraction helpers** for type-safe parameter access
4. **Return standardized response** using `SdkResponse.createSuccess()`

### Workflow Operations
Add new operations to the existing WorkflowOperations class/module:

1. **Add operation case** to the switch/if-else statement
2. **Create operation method** with proper error handling
3. **Use parameter extraction helpers** for type-safe parameter access
4. **Return standardized response** using `SdkResponse.createSuccess()`

## 🆕 Adding New Resource Types

### Step 1: Create Resource Operations
Create a new operations file following the naming convention:
- **C#**: `[Resource]Operations.cs`
- **Java**: `[Resource]Operations.java`
- **Python**: `[resource]_operations.py`

### Step 2: Implement Standard Interface
All resource operations must implement:
- **Execute method**: Main entry point with error handling
- **Operation routing**: Switch/if-else for operation selection
- **Parameter handling**: Type-safe parameter extraction
- **Response formatting**: Standardized JSON response

### Step 3: Update Main Entry Point
Add routing for the new resource type in the main entry point:
- **C#**: Update `Program.cs` resource switch
- **Java**: Update `Main.java` resource routing
- **Python**: Update `main.py` resource handling

## 📋 Standard Patterns

### Operation Method Signature
```csharp
// C#
private static SdkResponse NewOperation(Dictionary<string, JToken> parameters, ResourceApi api)

// Java
private static SdkResponse newOperation(Map<String, Object> parameters, ResourceClient api)

// Python
def _new_operation(parameters: Dict[str, Any], api: ResourceApi) -> SdkResponse:
```

### Parameter Extraction
```csharp
// C#
var param1 = parameters.GetString("param1");
var param2 = parameters.GetBool("param2");

// Java
String param1 = (String) parameters.get("param1");
Boolean param2 = (Boolean) parameters.get("param2");

// Python
param1 = parameters.get("param1", "")
param2 = parameters.get("param2", False)
```

### Response Creation
```csharp
// C#
return SdkResponse.CreateSuccess(result);

// Java
return SdkResponse.createSuccess(result);

// Python
return SdkResponse.create_success(result)
```

### Error Handling
```csharp
// C#
try
{
    var result = api.SomeOperation(parameters);
    return SdkResponse.CreateSuccess(result);
}
catch (Exception ex)
{
    return SdkResponse.CreateError($"Operation failed: {ex.Message}");
}

// Java
try {
    Object result = api.someOperation(parameters);
    return SdkResponse.createSuccess(result);
} catch (Exception ex) {
    return SdkResponse.createError("Operation failed: " + ex.getMessage());
}

// Python
try:
    result = api.some_operation(parameters)
    return SdkResponse.create_success(result)
except Exception as ex:
    return SdkResponse.create_error(f"Operation failed: {str(ex)}")
```

## 🔄 Operation Routing

### Main Entry Point Updates
Add new operation cases to the routing logic:

```csharp
// C# - Program.cs
switch (resource.ToLower())
{
    case "event":
        return EventOperations.Execute(operation, parameters);
    case "workflow":
        return WorkflowOperations.Execute(operation, parameters);
    case "newresource":  // Add new resource
        return NewResourceOperations.Execute(operation, parameters);
    default:
        return SdkResponse.CreateError($"Unknown resource: {resource}");
}
```

```java
// Java - Main.java
switch (resource.toLowerCase()) {
    case "event":
        return EventOperations.execute(operation, parameters);
    case "workflow":
        return WorkflowOperations.execute(operation, parameters);
    case "newresource":  // Add new resource
        return NewResourceOperations.execute(operation, parameters);
    default:
        return SdkResponse.createError("Unknown resource: " + resource);
}
```

```python
# Python - main.py
if resource.lower() == "event":
    return EventOperations.execute(operation, parameters)
elif resource.lower() == "workflow":
    return WorkflowOperations.execute(operation, parameters)
elif resource.lower() == "newresource":  # Add new resource
    return NewResourceOperations.execute(operation, parameters)
else:
    return SdkResponse.create_error(f"Unknown resource: {resource}")
```

## 🧪 Testing New Operations

### 1. Test CLI Wrapper Directly
```bash
# C#
dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- \
  --operation new-operation \
  --parameters '{"param1":"value1","param2":true}' \
  --resource event

# Java
java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar \
  --operation new-operation \
  --parameters '{"param1":"value1","param2":true}' \
  --resource event

# Python
python -m sdk_wrapper.main \
  --operation new-operation \
  --parameters '{"param1":"value1","param2":true}' \
  --resource event
```

### 2. Add Integration Test
```csharp
[Fact]
public async Task SdkIntegration_NewOperation_ValidatesAgainstApi()
{
    var parameters = new Dictionary<string, object>
    {
        ["param1"] = "test_value",
        ["param2"] = true
    };

    var sdkResponse = await ExecuteSdkCallAsync<ExpectedResponse>("new-operation", parameters, "event");
    var apiResponse = EventResourceApi.NewOperation(request);

    Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
    Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse));
}
```

## 📚 Related Documentation

- **[Main README](../README.md)** - Project overview and quick start
- **[SDK Integration Guide](../SDK_INTEGRATION_GUIDE.md)** - Technical architecture details
- **[Shell Scripts Reference](../SCRIPTS_README.md)** - Build and test automation
- **[CLI Wrapper READMEs](../SdkTestAutomation.CliWrappers/)** - Language-specific wrapper documentation 