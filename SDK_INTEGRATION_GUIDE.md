# SDK Integration Guide

This guide explains how the .NET test harness can call each SDK wrapper and parse the JSON output for validation.

## Overview

The SdkTestAutomation framework provides a unified way to test multiple Conductor SDKs (C#, Java, Python) through CLI wrappers. Each test runs against one chosen SDK per execution, and the framework validates that SDK responses match direct API responses.

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

The .NET test harness communicates with SDK wrappers via JSON over standard I/O:

**Input to CLI Wrapper:**
```json
{
  "operation": "add-event",
  "parameters": {
    "name": "test_event",
    "event": "test_event", 
    "active": true
  },
  "resource": "event"
}
```

**Output from CLI Wrapper:**
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

**Command:** `dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp`

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

**Command:** `java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar`

**Dependencies:**
- `conductor-client` Maven dependency
- `picocli` for CLI parsing
- `jackson-databind` for JSON serialization

**Build:** `mvn clean package`

**Example Usage:**
```bash
java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

### Python Wrapper

**Location:** `SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/`

**Command:** `python SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/sdk_wrapper/main.py`

**Dependencies:**
- `conductor-python` PyPI package
- `argparse` for CLI parsing (built-in)
- `json` for JSON serialization (built-in)

**Install:** `pip install -e .`

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
4. **Python SDK:** Python 3.8+ and pip

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
   private string GetWrapperPath(string sdkType) => sdkType.ToLowerInvariant() switch
   {
       "csharp" => "dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp",
       "java" => "java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar",
       "python" => "python SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/sdk_wrapper/main.py",
       "go" => "go run SdkTestAutomation.CliWrappers/SdkTestAutomation.Go/main.go",
       _ => throw new ArgumentException($"Unsupported SDK type: {sdkType}")
   };
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

2. **JSON Parsing Errors:**
   - Verify the wrapper outputs valid JSON
   - Check the `SdkResponse` structure matches

3. **Response Mismatch:**
   - Compare SDK and API response structures
   - Check for differences in data serialization

### Debug Mode

Enable detailed logging by setting the log level:

```bash
LOG_LEVEL=Debug SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

This will show the exact commands being executed and responses received. 