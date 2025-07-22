# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A .NET test automation framework for validating multiple Conductor SDKs (C#, Java, Python) through in-process adapters. Built with xUnit v3, RestSharp, and a language-agnostic architecture.

## 🎯 Key Features

- **Multi-SDK Support**: Test C#, Java, and Python Conductor SDKs with a single test codebase
- **In-Process Adapters**: Direct SDK integration without CLI overhead
- **Direct Executable**: Tests run as standalone executable, not through `dotnet test`
- **Type Safety**: Strong typing with C# interfaces and compile-time validation
- **Environment Configuration**: .env file support for flexible configuration
- **Response Validation**: Deep structural comparison of SDK responses with direct API calls
- **Extensible Architecture**: Easy to add new SDKs by implementing shared interfaces

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    .NET Test Framework                      │
│                     (xUnit v3 + RestSharp)                 │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                 SDK Adapter Interface                      │
│              (IEventResourceAdapter, etc.)                │
└─────────────────────┬───────────────────────────────────────┘
                      │
        ┌─────────────┼─────────────┐
        ▼             ▼             ▼
┌─────────────┐ ┌─────────────┐ ┌─────────────┐
│   C# SDK    │ │  Java SDK   │ │ Python SDK  │
│   Adapter   │ │   Adapter   │ │   Adapter   │
│ (Direct)    │ │(JCOBridge)  │ │(pythonnet)  │
└─────────────┘ └─────────────┘ └─────────────┘
```

## 📁 Project Structure

```
SdkTestAutomation/
├── SdkTestAutomation.Common/           # Shared interfaces & models
│   ├── Models/                         # Request/response models
│   ├── Interfaces/                     # SDK adapter interfaces
│   └── Helpers/                        # Factory & utilities
├── SdkTestAutomation.CSharp/           # C# SDK adapter
├── SdkTestAutomation.Java/             # Java SDK adapter (MASES.JCOBridge)
├── SdkTestAutomation.Python/           # Python SDK adapter (pythonnet)
├── SdkTestAutomation.Tests/            # Test implementations
├── SdkTestAutomation.Api/              # Direct API client
├── SdkTestAutomation.Core/             # Core HTTP functionality
└── SdkTestAutomation.Utils/            # Utilities & logging
```

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK
- Java 17+ (for Java SDK testing)
- Python 3.9+ (for Python SDK testing)
- Docker (for Conductor server)

### Configuration

1. **Clone the repository**
   ```bash
   git clone https://github.com/evgeniykisel/SdkTestAutomation.git
   cd SdkTestAutomation
   ```

2. **Create environment configuration**
   ```bash
   # Create .env file in SdkTestAutomation.Tests/
   cat > SdkTestAutomation.Tests/.env << EOF
   CONDUCTOR_SERVER_URL=http://localhost:8080/api
   TEST_SDK=csharp
   JAVA_HOME=/usr/lib/jvm/java-17-openjdk
   PYTHON_HOME=/usr/local/bin/python3
   ENABLE_LOGGING=true
   LOG_LEVEL=Info
   EOF
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

### Running Tests

#### **Direct Execution (Recommended)**
```bash
# Build the solution first
dotnet build

# Test with C# SDK
TEST_SDK=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Test with Java SDK
TEST_SDK=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Test with Python SDK
TEST_SDK=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Test with specific test filter
TEST_SDK=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests --filter "SdkIntegrationTests"

# Test with all SDKs (run multiple times with different TEST_SDK values)
TEST_SDK=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
TEST_SDK=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
TEST_SDK=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

#### **IDE Integration**
Set the `TEST_SDK` environment variable in your IDE (Rider/VS Code) and run the test executable directly.

## 📝 Writing Tests

### Base Test Class

All SDK integration tests inherit from `BaseTest`:

```csharp
public class SdkIntegrationTests : BaseTest
{
    [Fact]
    public async Task SdkIntegration_AddEvent_ValidatesAgainstApi()
    {
        // Arrange
        var request = new AddEventRequest
        {
            Name = $"test_event_{Guid.NewGuid():N}",
            Event = "test_event",
            Actions = new List<EventAction>(),
            Active = true
        };

        // Act - Call SDK via adapter
        var eventAdapter = await GetEventResourceAdapterAsync();
        var sdkResponse = await eventAdapter.AddEventAsync(request);

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
        
        LogAdapterInfo();
    }
}
```

### SDK Selection

The framework automatically selects the SDK based on the `TEST_SDK` environment variable:

- `TEST_SDK=csharp` - Uses C# SDK adapter
- `TEST_SDK=java` - Uses Java SDK adapter (requires MASES.JCOBridge)
- `TEST_SDK=python` - Uses Python SDK adapter (requires pythonnet)

## 🔧 SDK Adapters

### C# SDK Adapter
- **Direct Integration**: Uses `conductor-csharp` NuGet package
- **No Dependencies**: Pure .NET implementation
- **Full Type Safety**: Native C# objects and methods

### Java SDK Adapter
- **MASES.JCOBridge**: Modern .NET ⇄ JVM bridge
- **Dynamic Invocation**: Runtime Java object creation
- **JVM Management**: Automatic JVM lifecycle handling

### Python SDK Adapter
- **pythonnet**: .NET ⇄ Python bridge
- **Future Implementation**: Placeholder for Python SDK integration

## 🌍 Environment Configuration

The framework uses a `.env` file for configuration:

```env
# Conductor server
CONDUCTOR_SERVER_URL=http://localhost:8080/api

# SDK selection
TEST_SDK=csharp

# Java configuration
JAVA_HOME=/usr/lib/jvm/java-17-openjdk
JAVA_CLASSPATH=/path/to/conductor-java/lib/*

# Python configuration
PYTHON_HOME=/usr/local/bin/python3
PYTHONPATH=/path/to/conductor-python

# Logging
ENABLE_LOGGING=true
LOG_LEVEL=Info
```

## 🔍 Response Validation

The framework automatically validates SDK responses against direct API calls:

```csharp
// SDK response is automatically compared with API response
var isValid = await ValidateSdkResponseAsync(sdkResponse, apiResponse);
Assert.True(isValid, "SDK response does not match API response");
```

## 🚀 CI/CD Integration

### GitHub Actions

The framework includes comprehensive CI/CD workflows:

- **Build and Test**: Full multi-SDK testing using direct executable execution
- **Build**: Quick compilation validation
- **Test Reports**: HTML, TRX, and JUnit formats

### Local Development

```bash
# Build the solution
dotnet build

# Run tests with specific SDK
TEST_SDK=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
TEST_SDK=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
TEST_SDK=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 🔧 Troubleshooting

### Common Issues

1. **Java SDK Not Working**
   - Ensure `JAVA_HOME` is set correctly
   - Verify Java 17+ is installed
   - Check MASES.JCOBridge installation

2. **Python SDK Not Working**
   - Ensure `PYTHON_HOME` is set correctly
   - Verify Python 3.9+ is installed
   - Check pythonnet installation

3. **Environment Variables Not Loading**
   - Verify `.env` file exists in `SdkTestAutomation.Tests/`
   - Check file format (no spaces around `=`)
   - Ensure file is not ignored by `.gitignore`

### Debugging

Enable detailed logging by setting:
```env
ENABLE_LOGGING=true
LOG_LEVEL=Debug
```

## 🔄 Extending the Framework

### Adding a New SDK

1. **Create Adapter Project**
   ```bash
   dotnet new classlib -n SdkTestAutomation.NewSdk
   ```

2. **Implement Interfaces**
   ```csharp
   public class ConductorNewSdkEventResourceAdapter : IEventResourceAdapter
   {
       // Implement interface methods
   }
   ```

3. **Update Factory**
   ```csharp
   // Add to AdapterFactory.CreateEventResourceAdapterAsync
   "newsdk" => new ConductorNewSdkEventResourceAdapter(),
   ```

4. **Add Project Reference**
   ```xml
   <ProjectReference Include="..\SdkTestAutomation.NewSdk\SdkTestAutomation.NewSdk.csproj" />
   ```

## 📄 License

This project is licensed under the terms provided in the LICENSE file.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Implement your changes
4. Add tests for new functionality
5. Ensure all SDK tests pass
6. Submit a pull request

### Development Guidelines

- **Test All SDKs**: Ensure changes work with C#, Java, and Python
- **Maintain Compatibility**: Don't break existing adapter interfaces
- **Add Documentation**: Update README for new features
- **Follow Patterns**: Use existing adapter implementations as templates