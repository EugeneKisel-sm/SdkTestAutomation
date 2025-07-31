# C# SDK Integration

This directory contains the C# SDK integration for the SdkTestAutomation framework.

## 🎯 What is the C# SDK Integration?

The C# SDK integration provides native .NET support for testing Conductor SDKs. It directly uses the official Conductor C# SDK (conductor-csharp NuGet package) to interact with Conductor servers, offering the most straightforward and performant integration within the framework.

## 🚀 Why Was It Created?

### The Advantage
C# integration offers the most natural experience for .NET developers:
- **Native Performance**: Direct .NET-to-.NET communication
- **Type Safety**: Full IntelliSense and compile-time checking
- **Familiar APIs**: Uses standard .NET patterns and conventions
- **Zero Overhead**: No inter-process communication or serialization costs
- **Seamless Integration**: Works directly with existing .NET applications

### The Solution
The C# integration provides:
- **Direct SDK Usage**: Uses conductor-csharp NuGet package directly
- **Unified Interface**: Implements the same interfaces as other language adapters
- **Native Error Handling**: Leverages .NET exception handling
- **Performance Optimization**: Minimal abstraction layers

## 🔧 How It Works

### Architecture Overview

```
.NET Test Framework
    ↓ (Direct method calls)
C# SDK Adapters
    ↓ (HTTP calls via conductor-csharp)
Conductor Server
```

### Communication Flow

1. **Test Execution**: .NET test calls C# adapter directly
2. **SDK Invocation**: Adapter uses conductor-csharp SDK methods
3. **HTTP Communication**: SDK handles HTTP requests to Conductor
4. **Response Processing**: SDK deserializes responses to .NET objects
5. **Unified Format**: Adapter converts to framework's SdkResponse format

## 📁 Directory Structure

```
CSharp/
├── CSharpClient.cs                  # Main client implementation
├── CSharpEventAdapter.cs            # Event operations adapter
├── CSharpWorkflowAdapter.cs         # Workflow operations adapter
├── CSharpTokenAdapter.cs            # Token operations adapter (Orkes)
└── README.md                        # This documentation
```

## 🚀 Getting Started

### Prerequisites

- **.NET 8.0+** (required)
- **conductor-csharp NuGet package** (automatically installed)

### 1. Verify Installation

The C# integration is automatically available when you set `SDK_TYPE=csharp`:

```bash
# From project root
SDK_TYPE=csharp ../../SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

### 2. Check Dependencies

The required NuGet packages are automatically included:
- `conductor-csharp` - Official Conductor C# SDK
- `RestSharp` - HTTP client library

## 📝 How to Use

### Basic Usage

The C# integration works automatically when you set `SDK_TYPE=csharp`:

```csharp
public class EventTests : BaseConductorTest
{
    [Fact]
    public void AddEvent_ShouldSucceed()
    {
        // This will use C# SDK when SDK_TYPE=csharp
        var response = EventAdapter.AddEvent("test_event", "test_type", true);
        Assert.True(response.Success);
    }
}
```

### What Happens Behind the Scenes

1. **SDK Selection**: Framework detects `SDK_TYPE=csharp`
2. **Adapter Creation**: Creates `CSharpEventAdapter` instance
3. **Direct Invocation**: Adapter calls conductor-csharp SDK methods directly
4. **HTTP Communication**: SDK handles HTTP requests to Conductor server
5. **Response Processing**: SDK responses converted to `SdkResponse` format

## 📊 Request/Response Format

### Direct SDK Usage

The C# integration uses the conductor-csharp SDK directly:

```csharp
// Event operations
var eventApi = new EventApi(conductorClient);
var result = await eventApi.AddEventHandlerAsync(eventHandler);

// Workflow operations
var workflowApi = new WorkflowApi(conductorClient);
var status = await workflowApi.GetExecutionStatusAsync(workflowId);
```

### Unified Response Format

All operations return the framework's `SdkResponse` format:

```csharp
public class SdkResponse
{
    public bool Success { get; set; }
    public string Content { get; set; }
    public string ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `CONDUCTOR_SERVER_URL` | Conductor server URL | `http://localhost:8080/api` |

### SDK Configuration

The C# integration automatically configures the conductor-csharp SDK:

```csharp
var configuration = new Configuration
{
    BasePath = serverUrl,
    Timeout = 30000
};

var conductorClient = new ConductorClient(configuration);
```

## 🎯 Use Cases

### For .NET Developers
- Test your .NET applications with Conductor integration
- Validate conductor-csharp SDK behavior
- Ensure compatibility with different Conductor versions
- Debug SDK integration issues

### For Cross-Language Teams
- Use C# as the reference implementation
- Compare other language SDKs against C# behavior
- Validate API consistency across languages

### For Performance Testing
- Benchmark other language SDKs against native C# performance
- Identify performance bottlenecks in other integrations
- Validate optimization efforts

## 🔧 Troubleshooting

### Common Issues

**NuGet Package Issues:**
```bash
# Restore NuGet packages
dotnet restore

# Check package references
dotnet list package
```

**Connection Issues:**
```bash
# Verify Conductor server is running
curl http://localhost:8080/api/health

# Check server URL configuration
echo $CONDUCTOR_SERVER_URL
```

**SDK Version Conflicts:**
```bash
# Check installed package versions
dotnet list package | grep conductor

# Update to latest version
dotnet add package conductor-csharp --version latest
```

### Debug Mode

Enable debug logging to see detailed SDK communication:

```csharp
// In your test setup
_logger.LogLevel = "Debug";
```

This will show:
- HTTP requests and responses
- SDK method calls
- Configuration details

## 🔄 Extending C# Integration

### Adding New Operations

1. **Add SDK Method**: Use conductor-csharp SDK method directly
2. **Update Adapter**: Add corresponding method in adapter class
3. **Add Tests**: Create tests for the new operation

## 📊 Performance Considerations

### Advantages
- **Native Performance**: Direct method calls with no overhead
- **Type Safety**: Compile-time checking and IntelliSense
- **Memory Efficiency**: No serialization/deserialization overhead
- **Connection Reuse**: HTTP connections managed by SDK
- **Exception Handling**: Native .NET exception handling

### Best Practices
- **Async Operations**: Use async/await for better performance
- **Connection Pooling**: SDK handles connection management
- **Error Handling**: Implement proper try-catch blocks
- **Resource Disposal**: Use `using` statements for proper cleanup

### Performance Comparison

| Aspect | C# Integration | Other Integrations |
|--------|----------------|-------------------|
| **Startup Time** | Instant | Process startup overhead |
| **Memory Usage** | Minimal | Additional process memory |
| **Network Overhead** | Direct HTTP | JSON serialization + HTTP |
| **Error Handling** | Native exceptions | JSON error parsing |
| **Development Experience** | Full IntelliSense | Limited IDE support |

## 🔗 Related Documentation

- [Main Framework README](../../../README.md) - Overview of the entire framework
- [Conductor C# SDK](https://github.com/Netflix/conductor/tree/main/client/csharp) - Official C# SDK documentation
- [conductor-csharp NuGet](https://www.nuget.org/packages/conductor-csharp/) - NuGet package information
- [RestSharp Documentation](https://restsharp.dev/) - HTTP client library docs 