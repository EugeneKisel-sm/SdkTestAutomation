# Adding Operations to Python CLI Wrapper

Quick guide for adding new operations to the Python CLI wrapper.

> **See also**: **[C# Adding Operations](../SdkTestAutomation.CSharp/ADDING_OPERATIONS.md)** | **[Java Adding Operations](../SdkTestAutomation.Java/ADDING_OPERATIONS.md)**

## 📁 Structure

```
SdkTestAutomation.Python/sdk_wrapper/
├── main.py                       # Main entry point
├── operation_utils.py            # Common utilities
├── sdk_response.py               # Response model
└── operations/                   # Operation modules
    ├── event_operations.py       # Event operations
    └── workflow_operations.py    # Workflow operations
```

## 🎯 Adding Event Operations

### Step 1: Add to event_operations.py

```python
# In operations/event_operations.py
def _execute_operation(operation: str, parameters: Dict[str, Any]) -> SdkResponse:
    api_client = create_sdk_configuration()
    event_api = EventResourceApi(api_client)
    
    if operation == "add-event":
        return _add_event(parameters, event_api)
    elif operation == "get-event":
        return _get_event(event_api)
    elif operation == "new-operation":
        return _new_operation(parameters, event_api)  # ← Add here
    else:
        raise ValueError(f"Unknown event operation: {operation}")

# Add the operation method
def _new_operation(parameters: Dict[str, Any], event_api: EventResourceApi) -> SdkResponse:
    """Add a new event operation"""
    param1 = parameters.get("param1", "")
    param2 = parameters.get("param2", False)
    
    result = event_api.your_new_method(param1, param2)
    return SdkResponse.create_success(result)
```

## 🎯 Adding Workflow Operations

### Step 1: Add to workflow_operations.py

```python
# In operations/workflow_operations.py
def _execute_operation(operation: str, parameters: Dict[str, Any]) -> SdkResponse:
    api_client = create_sdk_configuration()
    workflow_api = WorkflowResourceApi(api_client)
    
    if operation == "get-workflow":
        return _get_workflow(parameters, workflow_api)
    elif operation == "new-workflow-operation":
        return _new_workflow_operation(parameters, workflow_api)  # ← Add here
    else:
        raise ValueError(f"Unknown workflow operation: {operation}")

def _new_workflow_operation(parameters: Dict[str, Any], workflow_api: WorkflowResourceApi) -> SdkResponse:
    """Add a new workflow operation"""
    workflow_id = parameters.get("workflowId", "")
    action = parameters.get("action", "")
    
    result = workflow_api.your_new_method(workflow_id, action)
    return SdkResponse.create_success(result)
```

## 🎯 Adding New Resource Types

### Step 1: Create task_operations.py

```python
# Create operations/task_operations.py
#!/usr/bin/env python3

from typing import Dict, Any
from conductor.client.http.api.task_resource_api import TaskResourceApi
from ..operation_utils import execute_with_error_handling, create_sdk_configuration
from ..sdk_response import SdkResponse

def execute(operation: str, parameters: Dict[str, Any]) -> SdkResponse:
    """Execute task operations with centralized error handling"""
    return execute_with_error_handling(lambda: _execute_operation(operation, parameters))

def _execute_operation(operation: str, parameters: Dict[str, Any]) -> SdkResponse:
    """Execute the specific task operation"""
    api_client = create_sdk_configuration()
    task_api = TaskResourceApi(api_client)
    
    if operation == "get-task":
        return _get_task(parameters, task_api)
    elif operation == "update-task":
        return _update_task(parameters, task_api)
    else:
        raise ValueError(f"Unknown task operation: {operation}")

def _get_task(parameters: Dict[str, Any], task_api: TaskResourceApi) -> SdkResponse:
    """Get task by ID"""
    task_id = parameters.get("taskId", "")
    task = task_api.get_task(task_id)
    return SdkResponse.create_success(task)
```

### Step 2: Update main.py

```python
# In main.py
def execute_operation(operation: str, parameters: dict, resource: str) -> SdkResponse:
    """Execute the operation based on resource type"""
    if resource == "event":
        return event_operations.execute(operation, parameters)
    elif resource == "workflow":
        return workflow_operations.execute(operation, parameters)
    elif resource == "task":
        return task_operations.execute(operation, parameters)  # ← Add here
    else:
        raise ValueError(f"Unknown resource: {resource}")
```

### Step 3: Update __init__.py

```python
# In operations/__init__.py
from . import event_operations, workflow_operations, task_operations  # ← Add here
```

## 🧪 Testing

### Install and Test

```bash
# Install
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Python
pip install -e .

# Test new operation
python -m sdk_wrapper.main \
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

## 📋 Best Practices

- **Error Handling**: Use `execute_with_error_handling()` - no try-catch needed
- **Parameters**: Use `parameters.get(key, default_value)` for safe extraction
- **Responses**: Use `SdkResponse.create_success()` or `SdkResponse.create_error()`
- **Naming**: Operations in kebab-case, methods in snake_case with underscore prefix
- **Documentation**: Add docstrings and update README.md

## 🔧 Parameter Types

```python
# Common parameter types
name = parameters.get("name", "")
active = parameters.get("active", False)
version = parameters.get("version", 0)
value = parameters.get("value", 0.0)
items = parameters.get("items", [])
config = parameters.get("config", {})

# Safe extraction with type conversion
def get_string_parameter(parameters: Dict[str, Any], key: str, default: str = "") -> str:
    value = parameters.get(key, default)
    return str(value) if value is not None else default

def get_bool_parameter(parameters: Dict[str, Any], key: str, default: bool = False) -> bool:
    value = parameters.get(key, default)
    if isinstance(value, bool):
        return value
    elif isinstance(value, str):
        return value.lower() in ('true', '1', 'yes', 'on')
    return default
```

## 🔍 Example: Pause Workflow

```python
# Add to workflow_operations.py
elif operation == "pause-workflow":
    return _pause_workflow(parameters, workflow_api)

def _pause_workflow(parameters: Dict[str, Any], workflow_api: WorkflowResourceApi) -> SdkResponse:
    """Pause a workflow execution"""
    workflow_id = parameters.get("workflowId", "")
    reason = parameters.get("reason", "")
    
    workflow_api.pause_workflow(workflow_id, reason)
    return SdkResponse.create_success()
```

Test:
```bash
python -m sdk_wrapper.main \
  --operation pause-workflow \
  --parameters '{"workflowId":"test-workflow","reason":"Maintenance"}' \
  --resource workflow
``` 