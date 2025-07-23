# Java CLI Wrapper

Optimized CLI wrapper for testing the Java Conductor SDK with clean separation of concerns.

## 📁 Architecture

```
SdkTestAutomation.Java/src/main/java/com/conductor/sdkwrapper/
├── Main.java                     # Main entry point
├── OperationUtils.java           # Common utilities
├── SdkResponse.java              # Response model
└── operations/                   # Operation classes
    ├── EventOperations.java      # Event operations
    └── WorkflowOperations.java   # Workflow operations
```

## 🎯 Key Features

- **Separation of Concerns**: Event and workflow operations completely separated
- **Centralized Error Handling**: All operations wrapped with error handling utilities
- **Static Factory Methods**: Clean response creation with `SdkResponse.createSuccess()`
- **Simplified Architecture**: Direct SDK interaction without unnecessary layers

> **Note**: All CLI wrappers follow the same architecture pattern. See other wrapper READMEs for language-specific details.

## 🚀 Usage

### Build
```bash
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Java
mvn clean package
```

### Run Operations
```bash
# Event operations
java -jar target/sdk-wrapper-1.0.0.jar \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event

# Workflow operations
java -jar target/sdk-wrapper-1.0.0.jar \
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
- **[Python Wrapper](../SdkTestAutomation.Python/README.md)** - Python CLI wrapper documentation

## 🛠️ Dependencies

- `conductor-client` - Java Conductor SDK
- `picocli` - CLI argument parsing
- `jackson-databind` - JSON serialization 