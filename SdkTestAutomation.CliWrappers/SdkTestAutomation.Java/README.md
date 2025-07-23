# Java CLI Wrapper

CLI wrapper for testing the Java Conductor SDK with clean separation of concerns.

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

## 🎯 Key Features

- **Separation of Concerns**: Event and workflow operations completely separated
- **Centralized Error Handling**: All operations wrapped with error handling utilities
- **Static Factory Methods**: Clean response creation with `SdkResponse.createSuccess()`
- **Simplified Architecture**: Direct SDK interaction without unnecessary layers

## 🚀 Usage

### Build
```bash
mvn clean package -f SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/pom.xml
```

### Run Operations
```bash
# Event operations
java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event

# Workflow operations
java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar \
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

- **[Adding Operations](ADDING_OPERATIONS.md)** - Java specific implementation guide
- **[Universal Operations Guide](../../ADDING_OPERATIONS_GUIDE.md)** - Cross-language patterns and best practices
- **[C# Wrapper](../SdkTestAutomation.CSharp/README.md)** - C# CLI wrapper documentation
- **[Python Wrapper](../SdkTestAutomation.Python/README.md)** - Python CLI wrapper documentation

## 🛠️ Dependencies

- `conductor-client` - Java Conductor SDK
- `picocli` - CLI argument parsing
- `jackson` - JSON serialization 