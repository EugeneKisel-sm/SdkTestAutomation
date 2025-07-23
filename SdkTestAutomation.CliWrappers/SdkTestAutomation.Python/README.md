# Python CLI Wrapper

Optimized CLI wrapper for testing the Python Conductor SDK with clean separation of concerns.

## 📁 Architecture

```
SdkTestAutomation.Python/sdk_wrapper/
├── main.py                       # Main entry point
├── operation_utils.py            # Common utilities
├── sdk_response.py               # Response model
└── operations/                   # Operation modules
    ├── event_operations.py       # Event operations
    └── workflow_operations.py    # Workflow operations
```

## 🎯 Key Features

- **Separation of Concerns**: Event and workflow operations completely separated
- **Centralized Error Handling**: All operations wrapped with error handling utilities
- **Static Factory Methods**: Clean response creation with `SdkResponse.create_success()`
- **Simplified Architecture**: Direct SDK interaction without unnecessary layers

> **Note**: All CLI wrappers follow the same architecture pattern. See other wrapper READMEs for language-specific details.

## 🚀 Usage

### Install
```bash
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Python
pip install -e .
```

### Run Operations
```bash
# Event operations
python sdk_wrapper/main.py \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event

# Workflow operations
python sdk_wrapper/main.py \
  --operation get-workflow \
  --parameters '{"workflowId":"test-workflow"}' \
  --resource workflow
```

## 📋 Supported Operations

### Event Operations
- `add-event` - Create a new event handler
- `get-event` - Get all event handlers
- `get-event-by-name` - Get event handlers by name
- `update-event` - Update an existing event handler
- `delete-event` - Delete an event handler

### Workflow Operations
- `get-workflow` - Get workflow execution status

## 🔧 Environment Variables

- `CONDUCTOR_SERVER_URL` - Conductor server URL (default: http://localhost:8080/api)

## 📚 Documentation

- **[Adding Operations](ADDING_OPERATIONS.md)** - Complete guide for adding new operations
- **[C# Wrapper](../SdkTestAutomation.CSharp/README.md)** - C# CLI wrapper documentation
- **[Java Wrapper](../SdkTestAutomation.Java/README.md)** - Java CLI wrapper documentation

## 🛠️ Dependencies

- `conductor-python` - Python Conductor SDK
- `argparse` - CLI argument parsing (built-in)
- `json` - JSON serialization (built-in) 