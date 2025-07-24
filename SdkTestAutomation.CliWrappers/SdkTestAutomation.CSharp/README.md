# C# CLI Wrapper

CLI wrapper for testing the C# Conductor SDK.

## 🚀 Usage

### Build
```bash
dotnet build SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/SdkTestAutomation.CSharp.csproj
```

### Run Operations
```bash
# Event operations
dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event

# Workflow operations
dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- \
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

- **[Adding Operations](ADDING_OPERATIONS.md)** - C# specific implementation guide
- **[Universal Operations Guide](../../ADDING_OPERATIONS_GUIDE.md)** - Cross-language patterns and best practices
- **[Java Wrapper](../SdkTestAutomation.Java/README.md)** - Java CLI wrapper documentation
- **[Python Wrapper](../SdkTestAutomation.Python/README.md)** - Python CLI wrapper documentation