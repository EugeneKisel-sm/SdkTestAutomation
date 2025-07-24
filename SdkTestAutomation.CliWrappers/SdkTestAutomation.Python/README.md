# Python CLI Wrapper

CLI wrapper for testing the Python Conductor SDK.

## 🚀 Usage

### Build
```bash
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Python
python -m venv venv
source venv/bin/activate  # On Windows: venv\Scripts\activate
pip install -e .
```

### Run Operations
```bash
# Event operations
python -m sdk_wrapper.main \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event

# Workflow operations
python -m sdk_wrapper.main \
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

- **[Adding Operations](ADDING_OPERATIONS.md)** - Python specific implementation guide
- **[Universal Operations Guide](../../ADDING_OPERATIONS_GUIDE.md)** - Cross-language patterns and best practices
- **[C# Wrapper](../SdkTestAutomation.CSharp/README.md)** - C# CLI wrapper documentation
- **[Java Wrapper](../SdkTestAutomation.Java/README.md)** - Java CLI wrapper documentation

## 🛠️ Dependencies

- `conductor-client` - Python Conductor SDK
- `argparse` - CLI argument parsing
- `json` - JSON serialization 