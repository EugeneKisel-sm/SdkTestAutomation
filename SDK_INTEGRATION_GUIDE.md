# SDK Integration Guide

This guide explains how the .NET test harness validates multiple Conductor SDKs (C#, Java, Python) through CLI wrappers and compares their responses with direct REST API calls.

## Overview

The SdkTestAutomation framework provides a unified way to test multiple Conductor SDKs through CLI wrappers. Each test runs against one chosen SDK per execution, and the framework validates that SDK responses match direct API responses for structural equality.

## Architecture

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

## How It Works

### 1. SDK Selection

The framework determines which SDK to use via environment variable:

```bash
# Run tests with C# SDK
SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Run tests with Java SDK  
SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests

# Run tests with Python SDK
SDK_TYPE=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

### 2. CLI Wrapper Communication

The .NET test harness communicates with SDK wrappers via command-line arguments and JSON over standard I/O:

**Command-line arguments:**
```bash
--operation add-event --parameters '{"name":"test_event","event":"test_event","active":true}' --resource event
```

**JSON Output from CLI Wrapper:**
```json
{
  "statusCode": 200,
  "success": true,
  "data": {...},
  "content": "...",
  "errorMessage": ""
}
```

### 3. Response Validation

The framework compares SDK responses with direct API responses for structural equality:

```csharp
// Call SDK via CLI wrapper
var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("add-event", parameters, "event");

// Call API directly for comparison
var apiResponse = EventResourceApi.AddEvent(request);

// Validate responses match
var isValid = await ValidateSdkResponseAsync<GetEventResponse>(sdkResponse, apiResponse);
Assert.True(isValid, "SDK response does not match API response");
```

## CLI Wrapper Implementation

### C# Wrapper

**Location:** `SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/`

**Build:** `dotnet build SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/SdkTestAutomation.CSharp.csproj`

**Dependencies:** 
- `conductor-csharp` NuGet package
- `System.CommandLine` for CLI parsing
- `Newtonsoft.Json` for JSON serialization

**Example Usage:**
```bash
dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

### Java Wrapper

**Location:** `SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/`

**Build:** `mvn clean package`

**Dependencies:**
- `conductor-client` Maven dependency
- `picocli` for CLI parsing
- `jackson-databind` for JSON serialization

**Example Usage:**
```bash
java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

### Python Wrapper

**Location:** `SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/`

**Install:** `pip install -e .`

**Dependencies:**
- `conductor-python` PyPI package
- `argparse` for CLI parsing (built-in)
- `json` for JSON serialization (built-in)

**Example Usage:**
```bash
python SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/sdk_wrapper/main.py \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

## Test Implementation

### BaseTest Class

The `BaseTest` class provides SDK integration capabilities:

```csharp
public abstract class BaseTest : IDisposable
{
    private readonly SdkCommandExecutor _sdkCommandExecutor;
    private readonly ResponseComparer _responseComparer;
    
    protected BaseTest()
    {
        var sdkType = Environment.GetEnvironmentVariable("SDK_TYPE") ?? "csharp";
        _sdkCommandExecutor = new SdkCommandExecutor(sdkType, _logger);
        _responseComparer = new ResponseComparer(_logger);
    }
    
    /// <summary>
    /// Executes a call to the selected SDK via CLI wrapper
    /// </summary>
    protected async Task<SdkResponse<T>> ExecuteSdkCallAsync<T>(string operation, Dictionary<string, object> parameters, string resource)
    {
        return await _sdkCommandExecutor.ExecuteAsync<T>(operation, parameters, resource);
    }
    
    /// <summary>
    /// Validates that SDK response matches the direct API response
    /// </summary>
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
    // Arrange
    var eventName = $"test_event_sdk_{Guid.NewGuid():N}";
    var parameters = new Dictionary<string, object>
    {
        ["name"] = eventName,
        ["event"] = "test_event",
        ["active"] = true
    };

    // Act - Call SDK via CLI wrapper
    var sdkResponse = await ExecuteSdkCallAsync<GetEventResponse>("add-event", parameters, "event");
    
    // Act - Call API directly for comparison
    var request = new AddEventRequest
    {
        Name = eventName,
        Event = "test_event",
        Actions = new List<EventAction>(),
        Active = true
    };
    var apiResponse = EventResourceApi.AddEvent(request);

    // Assert
    Assert.True(sdkResponse.Success, $"SDK call failed: {sdkResponse.ErrorMessage}");
    Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
    
    // Validate that SDK response matches API response
    var isValid = await ValidateSdkResponseAsync<GetEventResponse>(sdkResponse, apiResponse);
    Assert.True(isValid, "SDK response does not match API response");
}
```

## Running Tests

### Using the Test Runner Script

```bash
# Run tests with all SDKs
./run-tests.sh

# Run tests with specific SDK
./run-tests.sh csharp
./run-tests.sh java
./run-tests.sh python
```

### Manual Commands

```bash
# C# SDK
SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests -filter "/SdkTestAutomation.Tests/SdkTestAutomation.Tests.Conductor.EventResource/SdkIntegrationTests"

# Java SDK (requires Maven build first)
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Java && mvn clean package && cd ../../..
SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests -filter "/SdkTestAutomation.Tests/SdkTestAutomation.Tests.Conductor.EventResource/SdkIntegrationTests"

# Python SDK (requires pip install first)
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Python && pip install -e . && cd ../../..
SDK_TYPE=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests -filter "/SdkTestAutomation.Tests/SdkTestAutomation.Tests.Conductor.EventResource/SdkIntegrationTests"
```

## Prerequisites

1. **Conductor Server:** Running at `http://localhost:8080`
2. **C# SDK:** .NET 8.0 runtime
3. **Java SDK:** Java 17+ and Maven
4. **Python SDK:** Python 3.9+ and pip

## Adding a New SDK

To add support for a new SDK (e.g., Go):

1. **Create CLI Wrapper:**
   ```
   SdkTestAutomation.CliWrappers/SdkTestAutomation.Go/
   ├── go.mod
   ├── go.sum
   └── main.go
   ```

2. **Update SdkCommandExecutor:**
   ```csharp
   private (string fileName, string arguments) GetProcessInfo(string command)
   {
       var projectRoot = GetProjectRoot();
       return _sdkType switch
       {
           "csharp" => (Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/bin/Debug/net8.0/SdkTestAutomation.CSharp"), command),
           "java" => ("java", $"-jar \"{Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar")}\" {command}"),
           "python" => (Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/venv/bin/python"), $"{Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/sdk_wrapper/main.py")} {command}"),
           "go" => ("go", $"run {Path.Combine(projectRoot, "SdkTestAutomation.CliWrappers/SdkTestAutomation.Go/main.go")} {command}"),
           _ => throw new ArgumentException($"Unsupported SDK type: {_sdkType}")
       };
   }
   ```

3. **Update Test Runner Script:**
   ```bash
   case $1 in
       "csharp"|"java"|"python"|"go")
           run_tests_with_sdk $1
           ;;
   ```

## Troubleshooting

### Common Issues

1. **SDK Wrapper Not Found:**
   - Ensure the wrapper is built/installed
   - Check the wrapper path in `SdkCommandExecutor`
   - Verify the executable exists at the expected location

2. **JSON Parsing Errors:**
   - Verify the wrapper outputs valid JSON
   - Check the `SdkResponse` structure matches
   - Ensure no extra output is mixed with JSON response

3. **Response Mismatch:**
   - Compare SDK and API response structures
   - Check for differences in data serialization
   - Verify field names and data types match

4. **Process Execution Errors:**
   - Check if the SDK runtime is available (Java, Python, .NET)
   - Verify environment variables are set correctly
   - Ensure Conductor server is running and accessible

### Debug Mode

Enable detailed logging by setting the log level:

```bash
LOG_LEVEL=Debug SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

This will show:
- Exact commands being executed
- Raw output from CLI wrappers
- JSON parsing details
- Response comparison results

### Environment Variables

Required environment variables:

```bash
# Conductor server URL
export CONDUCTOR_SERVER_URL=http://localhost:8080/api

# SDK type to test (csharp, java, python)
export SDK_TYPE=csharp

# Optional: Log level for debugging
export LOG_LEVEL=Debug
```

## Architecture Benefits

1. **Language Independence**: .NET tests can validate any language SDK
2. **Extensibility**: Adding new SDKs requires only a new CLI wrapper
3. **Consistency**: All SDKs are tested with the same test cases
4. **Isolation**: SDK issues don't affect the test framework
5. **Maintainability**: Single test codebase for all SDKs
6. **CI/CD Friendly**: Easy to integrate into automated pipelines

## Best Practices

1. **Always validate responses**: Use `ValidateSdkResponseAsync` to ensure SDK correctness
2. **Handle errors gracefully**: Check `sdkResponse.Success` before validation
3. **Use unique test data**: Generate unique names/IDs to avoid conflicts
4. **Test all SDKs**: Run tests against all supported SDKs before committing
5. **Keep wrappers simple**: CLI wrappers should only handle SDK communication, not business logic 