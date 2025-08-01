# Java SDK Integration

This directory contains the Java SDK integration for the SdkTestAutomation framework.

## 🎯 What is the Java SDK Integration?

The Java SDK integration allows you to test Conductor Java SDKs using the SdkTestAutomation framework. It provides a bridge between the .NET test framework and Java Conductor SDKs, enabling you to validate Java SDK behavior alongside other language SDKs.

## 🚀 Why Was It Created?

### The Challenge
When testing Conductor SDKs across multiple languages, Java presents unique challenges:
- **Language Barrier**: .NET and Java are different ecosystems
- **Runtime Isolation**: Java runs in JVM, .NET in CLR
- **Dependency Management**: Java uses Maven/Gradle, .NET uses NuGet
- **Cross-Platform Compatibility**: Need to work on Windows, macOS, and Linux

### The Solution
The Java integration uses a CLI-based approach:
- **Process Communication**: .NET invokes Java applications via command line
- **JSON Protocol**: Standardized data exchange between languages
- **Maven Build System**: Clean dependency management for Java components
- **Separated Applications**: Dedicated CLI apps for Conductor and Orkes operations
- **Base Class Architecture**: Shared functionality through `BaseJavaAdapter` and `BaseJavaClient`
- **Organized Structure**: Clear separation between Conductor and Orkes adapters

## 🔧 How It Works

### Architecture Overview

```
.NET Test Framework
    ↓ (Process.Start)
Java CLI Applications
    ├── conductor-java-cli.jar (Conductor operations)
    └── orkes-java-cli.jar (Orkes operations)
    ↓ (HTTP calls)
Conductor/Orkes Server
```

### Communication Flow

1. **Test Execution**: .NET test calls Java adapter
2. **Process Launch**: Adapter starts Java CLI application
3. **JSON Request**: Parameters sent as JSON string
4. **Java Processing**: CLI app executes Conductor SDK operations
5. **JSON Response**: Results returned as JSON string
6. **Response Parsing**: .NET adapter converts JSON to unified format

## 📁 Directory Structure

```
Java/
├── cli-java-sdk/                    # Java CLI applications
│   ├── src/main/java/
│   │   └── com/sdktestautomation/
│   │       ├── ConductorCli.java    # Conductor operations CLI
│   │       ├── OrkesCli.java        # Orkes operations CLI
│   │       ├── BaseCli.java         # Shared CLI functionality
│   │       ├── models/
│   │       │   └── SdkResponse.java # Response model
│   │       ├── operations/
│   │       │   ├── conductor/       # Conductor operations
│   │       │   └── orkes/           # Orkes operations
│   │       └── utils/
│   │           └── OperationUtils.java
│   ├── pom.xml                      # Maven configuration
│   └── build.sh                     # Build script
├── Conductor/                       # Conductor adapters
│   ├── JavaClient.cs                # Conductor client wrapper
│   ├── JavaEventAdapter.cs          # Event operations adapter
│   └── JavaWorkflowAdapter.cs       # Workflow operations adapter
├── Orkes/                           # Orkes adapters
│   ├── JavaClient.cs                # Orkes client wrapper
│   └── JavaTokenAdapter.cs          # Token operations adapter
├── BaseJavaClient.cs                # Shared client functionality
├── BaseJavaAdapter.cs               # Shared adapter functionality
├── JavaResponse.cs                  # Response model
└── README.md                        # This documentation
```

## 🚀 Getting Started

### Prerequisites

- **Java 17+** installed and in PATH
- **Maven** installed for building Java applications
- **.NET 8.0+** (handled by main framework)

### 1. Build Java CLI Applications

```bash
cd SdkTestAutomation.Sdk/Implementations/Java/cli-java-sdk
./build.sh
```

This creates:
- `conductor-java-cli.jar` - For Conductor operations
- `orkes-java-cli.jar` - For Orkes operations

### 2. Verify Installation

```bash
# Test Conductor CLI
java -jar conductor-java-cli.jar --help

# Test Orkes CLI
java -jar orkes-java-cli.jar --help
```

### 3. Run Tests

```bash
# From project root
SDK_TYPE=java ../../SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 📝 How to Use

### Basic Usage

The Java integration works automatically when you set `SDK_TYPE=java`:

```csharp
public class EventTests : BaseConductorTest
{
    [Fact]
    public void AddEvent_ShouldSucceed()
    {
        // This will use Java SDK when SDK_TYPE=java
        var response = EventAdapter.AddEvent("test_event", "test_type", true);
        Assert.True(response.Success);
    }
}
```

### What Happens Behind the Scenes

1. **SDK Selection**: Framework detects `SDK_TYPE=java`
2. **Adapter Creation**: Creates adapter instance (inherits from `BaseJavaAdapter`)
3. **Base Class Initialization**: `BaseJavaAdapter` initializes Java client
4. **CLI Invocation**: Adapter calls `java -jar conductor-java-cli.jar`
5. **JSON Communication**: Request/response exchanged via JSON
6. **Response Processing**: JSON converted to `SdkResponse` object via base class

## 📊 Request/Response Format

### Request Format

```json
{
  "resource": "event",
  "operation": "add-event",
  "parameters": {
    "name": "test_event",
    "eventType": "test_event_type",
    "active": true
  }
}
```

### Response Format

```json
{
  "success": true,
  "data": "Event added successfully",
  "content": "Event added successfully",
  "statusCode": 200
}
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `CONDUCTOR_SERVER_URL` | Conductor server URL | `http://localhost:8080/api` |
| `JAVA_HOME` | Java installation path | Auto-detected |

### JAR File Locations

The build script copies JAR files to:
- `SdkTestAutomation.Sdk/bin/Debug/net8.0/lib/conductor-client.jar`
- `SdkTestAutomation.Sdk/bin/Debug/net8.0/lib/orkes-conductor-client.jar`

## 🎯 Use Cases

### For Java Developers
- Test your Java Conductor SDK implementation
- Validate SDK behavior against official API
- Ensure compatibility with different Conductor versions

### For Cross-Language Teams
- Compare Java SDK behavior with other language SDKs
- Ensure consistent API responses across languages
- Validate multi-language application compatibility

### For DevOps Teams
- Automated testing of Java SDK deployments
- Integration testing with Java microservices
- Performance validation of Java SDK operations

## 🔧 Troubleshooting

### Common Issues

**Java Not Found:**
```bash
# Check Java installation
java -version

# Set JAVA_HOME if needed
export JAVA_HOME=/path/to/java
```

**Maven Not Found:**
```bash
# Install Maven (macOS)
brew install maven

# Install Maven (Linux)
sudo apt-get install maven

# Check Maven installation
mvn -version
```

**JAR Files Missing:**
```bash
# Rebuild Java applications
cd SdkTestAutomation.Sdk/Implementations/Java/cli-java-sdk
./build.sh

# Verify JAR files exist
ls -la SdkTestAutomation.Sdk/bin/Debug/net8.0/lib/*.jar
```

### Debug Mode

Enable debug logging to see detailed communication:

```csharp
// In your test setup
_logger.LogLevel = "Debug";
```

This will show:
- CLI commands being executed
- JSON requests and responses
- Process start/stop information

## 🔄 Extending Java Integration

### Adding New Operations

1. **Create New Adapter**: Inherit from `BaseJavaAdapter`
2. **Add Java Operation**: Create new method in appropriate `Operations.java`
3. **Update CLI**: Add operation handling in `ConductorCli.java` or `OrkesCli.java`
4. **Update C# Adapter**: Add corresponding method using `ExecuteCall`
5. **Add Tests**: Create tests for the new operation

### Example: Adding a New Adapter

```csharp
public class JavaTaskAdapter : BaseJavaAdapter, ITaskAdapter
{
    public SdkResponse GetTask(string taskId)
    {
        return ExecuteCall("task", "get-task", new { TaskId = taskId });
    }
    
    public SdkResponse UpdateTask(string taskId, object taskData)
    {
        return ExecuteCall("task", "update-task", new { TaskId = taskId, Data = taskData });
    }
}
```

## 📊 Performance Considerations

### Advantages
- **Language Independence**: No runtime dependencies between .NET and Java
- **Clean Separation**: Java and .NET code are completely isolated
- **Flexible Deployment**: JAR files can be deployed independently
- **Version Compatibility**: Supports any Java version 17+

### Trade-offs
- **Process Overhead**: Each operation starts a new Java process
- **Serialization Cost**: JSON serialization/deserialization overhead
- **Memory Usage**: JVM startup for each operation

### Optimization Tips
- **Batch Operations**: Group multiple operations when possible
- **Connection Reuse**: Java SDK maintains HTTP connections internally
- **Error Handling**: Implement proper error handling to avoid hanging processes

## 🔗 Related Documentation

- [Main Framework README](../../../README.md) - Overview of the entire framework
- [Conductor Documentation](https://conductor.netflix.com/) - Official Conductor docs
- [Orkes Documentation](https://orkes.io/) - Orkes platform documentation

## 🆕 Recent Improvements

### Code Organization
- **Base Class Architecture**: All adapters now inherit from `BaseJavaAdapter` for consistent functionality
- **Organized Structure**: Clear separation between Conductor and Orkes adapters in dedicated folders
- **Shared Components**: Common client and response handling through base classes
- **Reduced Duplication**: Eliminated repetitive code across all Java adapters

### Benefits
- **Maintainability**: Changes to Java integration only need to be made in base classes
- **Consistency**: All adapters use the same error handling and client management patterns
- **Extensibility**: Easy to add new adapters by inheriting from base classes
- **Readability**: Cleaner, more focused adapter implementations 