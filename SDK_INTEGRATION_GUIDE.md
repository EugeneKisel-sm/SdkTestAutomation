# SDK Integration Guide

Quick guide for understanding how the .NET test framework validates multiple Conductor SDKs through CLI wrappers.

> **See also**: **[Main README](README.md)** for project overview and quick start.

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   .NET Tests    │    │   .NET Tests    │    │   .NET Tests    │
│   (xUnit v3)    │    │   (xUnit v3)    │    │   (xUnit v3)    │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          ▼                      ▼                      ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  C# CLI Wrapper │    │ Java CLI Wrapper│    │Python CLI Wrapper│
│  (dotnet run)   │    │  (java -jar)    │    │  (python -m)    │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          ▼                      ▼                      ▼
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   C# Conductor  │    │  Java Conductor │    │ Python Conductor│
│      SDK        │    │      SDK        │    │      SDK        │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🔄 How It Works

### 1. SDK Selection
Environment variable determines which SDK to test:
```bash
SDK_TYPE=csharp  # or java, python
```

### 2. CLI Communication
.NET tests communicate with SDK wrappers via JSON:
```bash
--operation add-event --parameters '{"name":"test","event":"test_event","active":true}' --resource event
```

### 3. Response Validation
Framework compares SDK responses with direct API calls:
```csharp
var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("add-event", parameters, "event");
var apiResponse = EventResourceApi.AddEvent(request);
var isValid = await ValidateSdkResponseAsync(sdkResponse, apiResponse);
```

## 🧪 Test Implementation

### BaseTest Class
```csharp
public abstract class BaseTest : IDisposable
{
    protected async Task<SdkResponse<T>> ExecuteSdkCallAsync<T>(string operation, Dictionary<string, object> parameters, string resource)
    {
        return await _sdkCommandExecutor.ExecuteAsync<T>(operation, parameters, resource);
    }
    
    protected async Task<bool> ValidateSdkResponseAsync<T>(SdkResponse<T> sdkResponse, RestResponse<T> apiResponse)
    {
        return await _responseComparer.CompareAsync(sdkResponse, apiResponse);
    }
}
```

### Example Test
```csharp
[Fact]
public async Task SdkIntegration_AddEvent_ValidatesAgainstApi()
{
    var parameters = new Dictionary<string, object>
    {
        ["name"] = $"test_event_{Guid.NewGuid():N}",
        ["event"] = "test_event",
        ["active"] = true
    };

    var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("add-event", parameters, "event");
    var apiResponse = EventResourceApi.AddEvent(request);

    Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
    Assert.True(await ValidateSdkResponseAsync(sdkResponse, apiResponse));
}
```

## 🚀 Running Tests

See **[Main README](README.md#🚀-quick-start)** for quick start instructions.

### Using Test Runner
```bash
# Validate environment first
./run-tests.sh --validate

# Run all SDKs
./run-tests.sh

# Run specific SDK
./run-tests.sh csharp
./run-tests.sh java
./run-tests.sh python

# Get help
./run-tests.sh --help
```

### Manual Commands
```bash
# C# SDK
SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Java SDK
SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Python SDK
SDK_TYPE=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 🔧 CLI Wrappers

### C# Wrapper
```bash
dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

### Java Wrapper
```bash
java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

### Python Wrapper
```bash
python -m sdk_wrapper.main \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

## 🔄 Adding New SDK

1. **Create CLI wrapper** following established patterns
2. **Update SdkCommandExecutor** to support new SDK
3. **Update test runner script** to include new SDK

Example for Go SDK:
```csharp
// In SdkCommandExecutor.cs
"go" => ("go", $"run {Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.Go/main.go")} {command}")
```

## 🐛 Troubleshooting

### Common Issues
- **SDK Wrapper Not Found**: Ensure wrapper is built/installed
- **JSON Parsing Errors**: Verify wrapper outputs valid JSON
- **Response Mismatch**: Compare SDK and API response structures

### Debug Mode
```bash
LOG_LEVEL=Debug SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 🔧 Environment Variables

The project uses environment variables for configuration. See **[env.template](../SdkTestAutomation.Tests/env.template)** for all available options.

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

## 📋 Prerequisites

- **Conductor Server**: Running at `http://localhost:8080`
- **C# SDK**: .NET 8.0 runtime
- **Java SDK**: Java 17+ and Maven
- **Python SDK**: Python 3.9+ and pip

## 🎯 Benefits

- **Language Independence**: .NET tests validate any language SDK
- **Extensibility**: Easy to add new SDKs with CLI wrappers
- **Consistency**: Same test cases for all SDKs
- **Isolation**: SDK issues don't affect test framework
- **CI/CD Friendly**: Easy integration into automated pipelines 