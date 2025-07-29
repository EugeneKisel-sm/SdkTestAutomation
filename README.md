# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A .NET test automation framework for validating multiple Conductor SDKs (C#, Java, Python, Go) through in-process adapters.

## 🎯 Key Features

- **Multi-SDK Support**: Test C#, Java, Python, and Go Conductor SDKs with a single test codebase
- **In-Process Adapters**: Direct SDK integration without CLI overhead
- **Response Validation**: Deep structural comparison of SDK responses with direct API calls
- **Extensible Architecture**: Easy to add new SDKs by implementing shared interfaces
- **Cross-Platform**: Supports Windows, macOS, and Linux with automatic platform detection

## 📁 Project Structure

```
SdkTestAutomation/
├── SdkTestAutomation.Sdk/           # Shared interfaces & models
│   ├── Implementations/
│   │   ├── CSharp/                  # C# SDK adapter
│   │   ├── Java/                    # Java SDK adapter (IKVM.NET)
│   │   ├── Python/                  # Python SDK adapter (Python.NET)
│   │   └── Go/                      # Go SDK adapter (Shared Library)
│   └── lib/                         # JAR files for Java SDK
├── SdkTestAutomation.Tests/         # Test implementations
├── SdkTestAutomation.Api/           # Direct API client
├── SdkTestAutomation.Core/          # Core HTTP functionality
├── SdkTestAutomation.Utils/         # Utilities & logging
└── setup-sdks.sh                    # Comprehensive setup script
```

## 🚀 Quick Start

### Prerequisites

- **.NET 8.0+** (required)
- **Java 17+** (optional, for Java SDK testing)
- **Python 3.9+** (optional, for Python SDK testing)
- **Go 1.19+** (optional, for Go SDK testing) with CGO enabled

### Automated Setup

```bash
# Clone the repository
git clone https://github.com/evgeniykisel/SdkTestAutomation.git
cd SdkTestAutomation

# Run comprehensive setup script
chmod +x setup-sdks.sh
./setup-sdks.sh
```

The setup script automatically:
- ✅ Checks all prerequisites
- ✅ Sets up C# SDK (conductor-csharp package)
- ✅ Sets up Java SDK (downloads JAR files, IKVM.NET bridge)
- ✅ Sets up Python SDK (installs conductor-python, Python.NET bridge)
- ✅ Sets up Go SDK (creates go.mod, builds shared library)
- ✅ Creates environment configuration
- ✅ Tests overall build

### Manual Setup Options

```bash
# Setup only (default)
./setup-sdks.sh

# Test existing SDKs only
./setup-sdks.sh --test-only

# Build Go shared library only
./setup-sdks.sh --build-only

# Complete setup, test, and build
./setup-sdks.sh --full

# Show help
./setup-sdks.sh --help
```

### Configuration

1. **Environment File**: The script creates `SdkTestAutomation.Tests/.env` with:
   ```bash
   # Conductor Server Configuration (configurable)
   CONDUCTOR_SERVER_URL=http://localhost:8080/api
   
   # SDK Selection (csharp, java, python, go)
   SDK_TYPE=csharp
   
   # Go SDK Configuration (optional)
   GO_API_SERVER_PORT=8081
   ```

2. **Configurable URLs**: All URLs are configurable via environment variables:
   - `CONDUCTOR_SERVER_URL`: Main Conductor server URL (default: `http://localhost:8080/api`)
   - `GO_API_SERVER_PORT`: Go HTTP API server port (default: `8081`)

3. **Start Conductor Server**: Ensure your Conductor server is running

### Running Tests

```bash
# Build the project
dotnet build

# Run tests with different SDKs
SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=go ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Run specific test methods
SDK_TYPE=go ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests -method "*AddEventTests*"

# Run tests with different Conductor server URLs
CONDUCTOR_SERVER_URL=http://my-server:8080/api SDK_TYPE=go ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
CONDUCTOR_SERVER_URL=https://conductor.example.com/api SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 🔧 SDK Integration Details

| SDK | Status | Integration Method | Performance |
|-----|--------|-------------------|-------------|
| **C#** | ✅ Ready | Direct NuGet package | Native |
| **Java** | ✅ Ready | IKVM.NET bridge | High |
| **Python** | ✅ Ready | Python.NET bridge | High |
| **Go** | ✅ Ready | Shared Library (P/Invoke) | **50x faster** |

### Go SDK Performance Benefits

The Go SDK uses a shared library approach for optimal performance:
- **50x faster** than HTTP API approach
- **Direct memory access** via shared library
- **No process communication** overhead
- **Cross-platform support** (Windows, macOS, Linux)

## 📝 Writing Tests

All SDK integration tests inherit from `BaseTest`:

```csharp
public class SdkIntegrationTests : BaseTest
{
    [Fact]
    public async Task SdkIntegration_AddEvent_ValidatesAgainstApi()
    {
        var eventName = $"test_event_{Guid.NewGuid():N}";
        
        // Test SDK call
        var sdkResponse = EventAdapter.AddEvent(eventName, "test_event", true);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.Equal(HttpStatusCode.OK, sdkResponse.StatusCode);
    }
}
```

The framework automatically selects the SDK based on the `SDK_TYPE` environment variable.

## 🔧 Troubleshooting

### Common Issues

- **Java SDK**: Verify Java 17+ and JAR files in `SdkTestAutomation.Sdk/lib/`
- **Python SDK**: Check Python 3.9+ and `conductor-python` installation
- **Go SDK**: Verify Go 1.19+ and shared library build
- **Environment**: Ensure `.env` file exists in `SdkTestAutomation.Tests/`

### Go SDK Specific

- **Shared Library Not Found**: Run `./setup-sdks.sh --build-only`
- **CGO Issues**: Ensure `CGO_ENABLED=1` and proper build tools
- **Platform Detection**: Script automatically detects Apple Silicon vs Intel

### Debug Mode

```csharp
_logger.LogLevel = "Debug";
```

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