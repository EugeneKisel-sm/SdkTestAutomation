# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A comprehensive .NET test automation framework for validating Conductor SDKs across multiple programming languages.

## 🎯 What is SdkTestAutomation?

SdkTestAutomation is a unified testing framework that allows you to validate Conductor SDKs written in different programming languages (C#, Java, Python, Go) using a single test codebase. Instead of maintaining separate test suites for each SDK, you write tests once and run them against any supported SDK.

## 🚀 Why Was It Created?

### The Problem
When working with Conductor (a workflow orchestration platform), you often need to:
- Test SDKs in multiple programming languages
- Ensure consistent behavior across different SDK implementations
- Validate that SDK responses match the official API
- Maintain separate test suites for each language (time-consuming and error-prone)

### The Solution
SdkTestAutomation provides:
- **Single Test Codebase**: Write tests once, run against any SDK
- **Cross-Language Validation**: Ensure all SDKs behave consistently
- **API Compliance Testing**: Validate SDK responses against direct API calls
- **Unified Reporting**: Get consistent test results regardless of the SDK being tested

## 🔧 How It Works

### Architecture Overview

```
Your Test Code (C#)
    ↓
SdkTestAutomation Framework
    ↓
Language-Specific Adapters
    ├── C# SDK (Direct)
    ├── Java SDK (CLI)
    ├── Python SDK (Python.NET)
    └── Go SDK (Shared Library)
    ↓
Conductor Server
```

### Supported SDKs

| Language | Integration Method | Use Case |
|----------|-------------------|----------|
| **C#** | Direct NuGet package | Native .NET applications |
| **Java** | CLI applications | Java-based microservices |
| **Python** | Python.NET bridge | Python data processing |
| **Go** | Shared library | High-performance services |

## 📁 Project Structure

```
SdkTestAutomation/
├── SdkTestAutomation.Sdk/           # Core framework & adapters
│   ├── Core/                        # Shared interfaces & models
│   └── Implementations/             # Language-specific adapters
│       ├── CSharp/                  # C# SDK integration
│       ├── Java/                    # Java SDK integration
│       ├── Python/                  # Python SDK integration
│       └── Go/                      # Go SDK integration
├── SdkTestAutomation.Tests/         # Test implementations
├── SdkTestAutomation.Api/           # Direct API client
├── SdkTestAutomation.Core/          # HTTP client framework
├── SdkTestAutomation.Utils/         # Utilities & logging
└── setup-sdks.sh                    # Automated setup script
```

## 🚀 Quick Start

### Prerequisites

- **.NET 8.0+** (required)
- **Java 17+** (for Java SDK testing)
- **Maven** (for Java SDK building)
- **Python 3.9+** (for Python SDK testing)
- **Go 1.19+** (for Go SDK testing)

### 1. Clone and Setup

```bash
git clone https://github.com/evgeniykisel/SdkTestAutomation.git
cd SdkTestAutomation

# Run automated setup
chmod +x setup-sdks.sh
./setup-sdks.sh
```

The setup script automatically:
- ✅ Installs all required dependencies
- ✅ Builds language-specific SDK integrations
- ✅ Creates configuration files
- ✅ Validates the setup

### 2. Configure Your Environment

The setup creates `SdkTestAutomation.Tests/.env`:

```bash
# Conductor Server URL
CONDUCTOR_SERVER_URL=http://localhost:8080/api

# SDK to test (csharp, java, python, go)
SDK_TYPE=csharp
```

### 3. Start Conductor Server

Ensure your Conductor server is running (the framework will connect to it).

### 4. Run Tests

```bash
# Build the project
dotnet build

# Test with different SDKs
SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
SDK_TYPE=go ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Run specific test
SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests -filter "*AddEvent*"
```

## 📝 Writing Tests

### Basic Test Structure

All tests inherit from base classes that provide SDK adapters:

```csharp
public class EventTests : BaseConductorTest
{
    [Fact]
    public void AddEvent_ShouldSucceed()
    {
        // Arrange
        var eventName = $"test_event_{Guid.NewGuid():N}";
        
        // Act
        var response = EventAdapter.AddEvent(eventName, "test_event", true);
        
        // Assert
        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

### What Happens Behind the Scenes

1. **SDK Selection**: The framework automatically selects the SDK based on `SDK_TYPE`
2. **Adapter Creation**: Creates the appropriate language adapter
3. **API Validation**: Compares SDK response with direct API call
4. **Unified Response**: Returns consistent response format regardless of SDK

## 🔧 Configuration Options

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `CONDUCTOR_SERVER_URL` | Conductor server URL | `http://localhost:8080/api` |
| `SDK_TYPE` | SDK to test | `csharp` |

### Testing Different Environments

```bash
# Test against local development server
CONDUCTOR_SERVER_URL=http://localhost:8080/api SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Test against staging environment
CONDUCTOR_SERVER_URL=https://staging-conductor.example.com/api SDK_TYPE=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 🎯 Use Cases

### For SDK Developers
- Validate your SDK implementation against the official API
- Ensure consistent behavior across different versions
- Catch regressions before releasing

### For Application Developers
- Test your application with different SDKs
- Ensure your code works with multiple language implementations
- Validate SDK compatibility

### For DevOps Teams
- Automated testing of SDK deployments
- Cross-language compatibility validation
- Performance comparison across SDKs

## 🔧 Troubleshooting

### Common Issues

- **Java SDK**: Verify Java 17+, Maven, and run `./setup-sdks.sh`
- **Python SDK**: Check Python 3.9+ and `conductor-python` installation
- **Go SDK**: Verify Go 1.19+ and shared library build
- **Environment**: Ensure `.env` file exists in `SdkTestAutomation.Tests/`

### Getting Help

1. Check the logs for detailed error messages
2. Verify all prerequisites are installed
3. Ensure Conductor server is running and accessible
4. Run `./setup-sdks.sh` to reinstall dependencies

## 🔄 Extending the Framework

### Adding a New SDK

1. Create adapter project: `dotnet new classlib -n SdkTestAutomation.NewSdk`
2. Implement `IEventAdapter` and `IWorkflowAdapter` interfaces
3. Update `SdkFactory` to include the new SDK
4. Add project reference to `SdkTestAutomation.Tests`

## 📊 Performance Comparison

| SDK | Integration Method | Performance | Use Case |
|-----|-------------------|-------------|----------|
| **C#** | Direct | Native | .NET applications |
| **Java** | CLI | High | Java microservices |
| **Python** | Python.NET | High | Data processing |
| **Go** | Shared Library | **50x faster** | High-performance services |

The Go SDK uses a shared library approach for optimal performance, making it ideal for high-throughput scenarios.

## 📄 License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Implement your changes
4. Ensure all SDK tests pass
5. Submit a pull request