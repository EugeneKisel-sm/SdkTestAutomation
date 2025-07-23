# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A .NET test automation framework for validating multiple Conductor SDKs (C#, Java, Python) through CLI wrappers. Ensures SDK correctness by comparing responses with direct REST API calls.

## 🎯 Key Features

- **Multi-SDK Support**: Test C#, Java, and Python SDKs with single test codebase
- **CLI-First Architecture**: Language-agnostic approach using command-line wrappers
- **Deep Validation**: Compare SDK responses with direct REST API calls
- **Extensible Design**: Easy to add new SDKs with minimal changes

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK
- Java 17+ and Maven (for Java SDK)
- Python 3.9+ and pip (for Python SDK)
- Docker

### Setup & Run
```bash
# 1. Setup environment
./scripts/setup-env.sh --minimal

# 2. Start Conductor server
docker run -d -p 8080:8080 conductoross/conductor-server:latest

# 3. Run all SDK tests
./scripts/run-tests.sh

# 4. Or run specific SDK
./scripts/run-tests.sh csharp
./scripts/run-tests.sh java
./scripts/run-tests.sh python
```

> **📖 For detailed script documentation**: See **[Shell Scripts Reference](SCRIPTS_README.md)** for comprehensive explanations of all scripts, their options, and troubleshooting.

## 📁 Project Structure

```
SdkTestAutomation/
├── SdkTestAutomation.Core/           # HTTP client and request resolvers
├── SdkTestAutomation.Api/            # Direct API client
├── SdkTestAutomation.Sdk/            # SDK command executor and response comparer
├── SdkTestAutomation.CliWrappers/    # CLI wrappers for each SDK
│   ├── SdkTestAutomation.CSharp/     # C# wrapper
│   ├── SdkTestAutomation.Java/       # Java wrapper
│   └── SdkTestAutomation.Python/     # Python wrapper
├── SdkTestAutomation.Tests/          # Test implementations
├── SdkTestAutomation.Utils/          # Configuration and logging
└── scripts/                          # Build and test scripts
```

## 🧪 Writing Tests

```csharp
public class MyTests : BaseTest
{
    [Fact]
    public async Task MySdkTest()
    {
        var parameters = new Dictionary<string, object>
        {
            ["name"] = "test_event",
            ["event"] = "test_event",
            ["active"] = true
        };

        var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("add-event", parameters, "event");
        var apiResponse = EventResourceApi.AddEvent(request);

        Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
        Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse));
    }
}
```

## 🔧 CLI Wrappers

All wrappers follow the same architecture pattern. See individual wrapper READMEs for language-specific details:

- **[C# CLI Wrapper](SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/README.md)**
- **[Java CLI Wrapper](SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/README.md)**
- **[Python CLI Wrapper](SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/README.md)**

## 🔧 Environment Variables

Essential variables (see `env.template` for all options):
```bash
export CONDUCTOR_SERVER_URL=http://localhost:8080/api
export SDK_TYPE=csharp  # or java, python
```

## 📚 Documentation

- **[Shell Scripts Reference](SCRIPTS_README.md)** - Comprehensive guide to all shell scripts
- **[SDK Integration Guide](SDK_INTEGRATION_GUIDE.md)** - Detailed architecture and implementation guide
- **[Adding Operations Guide](ADDING_OPERATIONS_GUIDE.md)** - Universal guide for adding new operations
- **[Adding New SDK](SDK_INTEGRATION_GUIDE.md#🔄-adding-new-sdk)** - Instructions for adding new SDKs

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