# Go CLI Wrapper

CLI wrapper for testing the Go Conductor SDK.

## 🚀 Usage

### Build
```bash
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Go
go build -o go-sdk-wrapper
```

### Run Operations
```bash
# Event operations
./go-sdk-wrapper \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event

# Workflow operations
./go-sdk-wrapper \
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
- `CONDUCTOR_AUTH_KEY` - Authentication key (optional)
- `CONDUCTOR_AUTH_SECRET` - Authentication secret (optional)
- `CONDUCTOR_CLIENT_HTTP_TIMEOUT` - HTTP timeout in seconds (default: 30)

## 📚 Documentation

- **[Adding Operations](ADDING_OPERATIONS.md)** - Go specific implementation guide
- **[Universal Operations Guide](../../ADDING_OPERATIONS_GUIDE.md)** - Cross-language patterns and best practices
- **[C# Wrapper](../SdkTestAutomation.CSharp/README.md)** - C# CLI wrapper documentation
- **[Java Wrapper](../SdkTestAutomation.Java/README.md)** - Java CLI wrapper documentation
- **[Python Wrapper](../SdkTestAutomation.Python/README.md)** - Python CLI wrapper documentation 