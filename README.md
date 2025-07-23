# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A .NET test automation framework for validating multiple Conductor SDKs (C#, Java, Python) through CLI wrappers. Ensures SDK correctness by comparing responses with direct REST API calls.

## 🎯 Key Features

- **Multi-SDK Support**: Test C#, Java, and Python SDKs with single test codebase
- **CLI-First Architecture**: Language-agnostic approach using command-line wrappers
- **Deep Validation**: Compare SDK responses with direct REST API calls
- **Extensible Design**: Easy to add new SDKs with minimal changes

## 🏗️ Architecture

See **[SDK Integration Guide](SDK_INTEGRATION_GUIDE.md#🏗️-architecture)** for detailed architecture diagram and flow.

**Test Flow**: SDK Selection → Command Building → CLI Execution → Response Capture → API Comparison → Validation

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Java 17+ and Maven
- Python 3.9+ and pip
- Docker

### Environment Setup
1. **Quick setup (recommended):**
   ```bash
   ./setup-env.sh --minimal
   ```

2. **Full setup with all options:**
   ```bash
   ./setup-env.sh --full
   ```

3. **Manual setup:**
   ```bash
   cp SdkTestAutomation.Tests/env.example SdkTestAutomation.Tests/.env  # or cp SdkTestAutomation.Tests/env.template SdkTestAutomation.Tests/.env
   # Edit SdkTestAutomation.Tests/.env file with your settings
   ```

### Build CLI Wrappers

The CLI wrappers are automatically built when running tests, but you can also build them manually:

```bash
# Build all wrappers
./build-wrappers.sh

# Build specific wrappers
./build-wrappers.sh --csharp
./build-wrappers.sh --java
./build-wrappers.sh --python

# Clean build (remove existing artifacts)
./build-wrappers.sh --clean

# Verbose output
./build-wrappers.sh --verbose

# Get help
./build-wrappers.sh --help
```

### Run Tests

1. **Start Conductor server**
   ```bash
   docker run -d -p 8080:8080 conductoross/conductor-server:latest
   ```

2. **Validate environment** (optional)
   ```bash
   ./run-tests.sh --validate
   ```

3. **Run all SDK tests**
   ```bash
   ./run-tests.sh
   ```

4. **Run specific SDK**
   ```bash
   ./run-tests.sh csharp
   ./run-tests.sh java
   ./run-tests.sh python
   ```

5. **Get help**
   ```bash
   ./run-tests.sh --help
   ```

## 📁 Project Structure

```
SdkTestAutomation/
├── SdkTestAutomation.Common/           # Shared models and helpers
├── SdkTestAutomation.CliWrappers/      # CLI wrappers for each SDK
│   ├── SdkTestAutomation.CSharp/       # C# wrapper
│   ├── SdkTestAutomation.Java/         # Java wrapper
│   └── SdkTestAutomation.Python/       # Python wrapper
├── SdkTestAutomation.Tests/            # Test implementations
│   ├── env.template                     # Environment template
│   └── env.example                      # Environment example
├── SdkTestAutomation.Api/              # Direct API client
└── run-tests.sh                        # Multi-SDK test runner
```

## 🧪 Writing Tests

```csharp
public class MyTests : BaseTest
{
    [Fact]
    public async Task MySdkTest()
    {
        // Arrange
        var parameters = new Dictionary<string, object>
        {
            ["name"] = "test_event",
            ["event"] = "test_event",
            ["active"] = true
        };

        // Act - Call SDK via CLI wrapper
        var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("add-event", parameters, "event");
        
        // Act - Call API directly for comparison
        var apiResponse = EventResourceApi.AddEvent(request);

        // Assert
        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse), 
                   "SDK response does not match API response");
    }
}
```

## 🔧 CLI Wrappers

All wrappers follow the same optimized architecture. See **[SDK Integration Guide](SDK_INTEGRATION_GUIDE.md#🔧-cli-wrappers)** for detailed usage examples.

**Quick Examples:**
- **C#**: `dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- --operation add-event --parameters '{"name":"test"}' --resource event`
- **Java**: `java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar --operation add-event --parameters '{"name":"test"}' --resource event`
- **Python**: `python -m sdk_wrapper.main --operation add-event --parameters '{"name":"test"}' --resource event`

## 📚 Documentation

### Wrapper Documentation
- **[C# CLI Wrapper](SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/README.md)**
- **[Java CLI Wrapper](SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/README.md)**
- **[Python CLI Wrapper](SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/README.md)**

### Adding Operations
- **[C# Adding Operations](SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/ADDING_OPERATIONS.md)**
- **[Java Adding Operations](SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/ADDING_OPERATIONS.md)**
- **[Python Adding Operations](SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/ADDING_OPERATIONS.md)**

## 🔧 Environment Variables

The project uses environment variables for configuration. See **[env.template](SdkTestAutomation.Tests/env.template)** for all available options.

**Essential variables:**
```bash
export CONDUCTOR_SERVER_URL=http://localhost:8080/api
export SDK_TYPE=csharp  # or java, python
```

**Quick setup:**
```bash
cp SdkTestAutomation.Tests/env.example SdkTestAutomation.Tests/.env
# Edit SdkTestAutomation.Tests/.env file with your settings
```

## 🔄 Adding a New SDK

See **[SDK Integration Guide](SDK_INTEGRATION_GUIDE.md#🔄-adding-new-sdk)** for detailed instructions on adding new SDKs.

## 📊 Test Reports

Test results are available in multiple formats:
- **HTML**: Interactive report with detailed test information
- **TRX**: Native .NET test results format
- **JUnit XML**: Standard XML format for CI/CD integration

View results in GitHub Actions or download artifacts from workflow runs.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Ensure all tests pass locally for all SDKs
4. Create a Pull Request

## 📄 License

This project is licensed under the terms provided in the LICENSE file.