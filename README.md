# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A .NET test automation framework for validating multiple Conductor SDKs (C#, Java, Python, Go) through in-process adapters.

## 🎯 Key Features

- **Multi-SDK Support**: Test C#, Java, Python, and Go Conductor SDKs with a single test codebase
- **In-Process Adapters**: Direct SDK integration without CLI overhead
- **Response Validation**: Deep structural comparison of SDK responses with direct API calls
- **Extensible Architecture**: Easy to add new SDKs by implementing shared interfaces



## 📁 Project Structure

```
SdkTestAutomation/
├── SdkTestAutomation.Sdk/           # Shared interfaces & models
│   ├── Implementations/
│   │   ├── CSharp/                  # C# SDK adapter
│   │   ├── Java/                    # Java SDK adapter (IKVM.NET)
│   │   ├── Python/                  # Python SDK adapter (Python.NET)
│   │   └── Go/                      # Go SDK adapter (Process communication)
│   └── lib/                         # JAR files for Java SDK
├── SdkTestAutomation.Tests/         # Test implementations
├── SdkTestAutomation.Api/           # Direct API client
├── SdkTestAutomation.Core/          # Core HTTP functionality
└── SdkTestAutomation.Utils/         # Utilities & logging
```

## 🚀 Quick Start

### Prerequisites

- .NET 8.0 SDK
- Java 17+ (for Java SDK testing)
- Python 3.9+ (for Python SDK testing)
- Go 1.19+ (for Go SDK testing) with CGO enabled

### Quick Setup

```bash
# Run automated setup script
chmod +x setup-sdks.sh
./setup-sdks.sh

# Test Go SDK setup specifically
./test-go-sdk.sh
```

### Configuration

1. **Clone and setup**
   ```bash
   git clone https://github.com/evgeniykisel/SdkTestAutomation.git
   cd SdkTestAutomation
   ./setup-sdks.sh
   ```

2. **Configure environment**
   ```bash
   # Edit SdkTestAutomation.Tests/.env
   CONDUCTOR_SERVER_URL=http://localhost:8080/api
   SDK_TYPE=csharp
   ```

### Running Tests

```bash
# Build and run tests
dotnet build
SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=go ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 🔧 SDK-Specific Setup

### Go SDK Setup

The Go SDK uses a shared library approach for optimal performance:

```bash
# The setup script automatically:
# 1. Sets up Go module with conductor-go v1.5.4
# 2. Builds shared library (conductor-go-bridge.dll/.so/.dylib)
# 3. Integrates with .NET via P/Invoke

# Test Go SDK setup
./test-go-sdk.sh

# Run tests with Go SDK
SDK_TYPE=go dotnet test SdkTestAutomation.Tests
```

**Performance Benefits:**
- 50x faster than HTTP API approach
- Direct memory access via shared library
- No process communication overhead

**Requirements:**
- Go 1.19+ with CGO enabled
- conductor-go SDK v1.5.4
- Platform-specific shared library build

## 📝 Writing Tests

All SDK integration tests inherit from `BaseTest`:

```csharp
public class SdkIntegrationTests : BaseTest
{
    [Fact]
    public async Task SdkIntegration_AddEvent_ValidatesAgainstApi()
    {
        var request = new AddEventRequest
        {
            Name = $"test_event_{Guid.NewGuid():N}",
            Event = "test_event",
            Actions = new List<EventAction>(),
            Active = true
        };

        var eventAdapter = await GetEventResourceAdapterAsync();
        var sdkResponse = await eventAdapter.AddEventAsync(request);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(200, sdkResponse.StatusCode);
    }
}
```

The framework automatically selects the SDK based on the `SDK_TYPE` environment variable.

## 🔧 SDK Adapters

| SDK | Status | Integration Method |
|-----|--------|-------------------|
| **C#** | ✅ Ready | Direct NuGet package |
| **Java** | ✅ Ready | IKVM.NET bridge |
| **Python** | ✅ Ready | Python.NET bridge |
| **Go** | ✅ Ready | Shared Library |

## 📚 Documentation

- [SDK Integration Guide](SDK_INTEGRATION.md) - Detailed setup instructions
- [Official SDK Repositories](https://github.com/conductor-oss) - C#, Java, Python, Go SDKs





## 🔧 Troubleshooting

- **Java SDK**: Verify Java 17+ and JAR files in `SdkTestAutomation.Sdk/lib/`
- **Python SDK**: Check Python 3.9+ and `conductor-python` installation
- **Go SDK**: Verify Go 1.19+ and `conductor-go` module installation
- **Environment**: Ensure `.env` file exists in `SdkTestAutomation.Tests/`

See [SDK Integration Guide](SDK_INTEGRATION.md) for detailed troubleshooting.

## 🔄 Extending the Framework

To add a new SDK:

1. Create adapter project: `dotnet new classlib -n SdkTestAutomation.NewSdk`
2. Implement `IEventResourceAdapter` interface
3. Update `AdapterFactory.CreateEventResourceAdapterAsync`
4. Add project reference to `SdkTestAutomation.Tests`

## 📄 License

This project is licensed under the terms provided in the LICENSE file.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Implement your changes
4. Ensure all SDK tests pass
5. Submit a pull request