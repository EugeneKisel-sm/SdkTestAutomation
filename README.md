# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A .NET test automation framework for validating multiple Conductor SDKs (C#, Java, Python) through in-process adapters.

## 🎯 Key Features

- **Multi-SDK Support**: Test C#, Java, and Python Conductor SDKs with a single test codebase
- **In-Process Adapters**: Direct SDK integration without CLI overhead
- **Response Validation**: Deep structural comparison of SDK responses with direct API calls
- **Extensible Architecture**: Easy to add new SDKs by implementing shared interfaces



## 📁 Project Structure

```
SdkTestAutomation/
├── SdkTestAutomation.Common/           # Shared interfaces & models
├── SdkTestAutomation.CSharp/           # C# SDK adapter
├── SdkTestAutomation.Java/             # Java SDK adapter (IKVM.NET)
├── SdkTestAutomation.Python/           # Python SDK adapter (Python.NET)
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

### Quick Setup

```bash
# Run automated setup script
chmod +x setup-sdks.sh
./setup-sdks.sh
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
   TEST_SDK=csharp
   ```

### Running Tests

```bash
# Build and run tests
dotnet build
TEST_SDK=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
TEST_SDK=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
TEST_SDK=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

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

The framework automatically selects the SDK based on the `TEST_SDK` environment variable.

## 🔧 SDK Adapters

| SDK | Status | Integration Method |
|-----|--------|-------------------|
| **C#** | ✅ Ready | Direct NuGet package |
| **Java** | ✅ Ready | IKVM.NET bridge |
| **Python** | ✅ Ready | Python.NET bridge |

## 📚 Documentation

- [SDK Integration Guide](SDK_INTEGRATION.md) - Detailed setup instructions
- [Official SDK Repositories](https://github.com/conductor-oss) - C#, Java, Python SDKs





## 🔧 Troubleshooting

- **Java SDK**: Verify Java 17+ and JAR files in `SdkTestAutomation.Java/lib/`
- **Python SDK**: Check Python 3.9+ and `conductor-python` installation
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