# SDK Integration Guide

Technical guide for understanding how the .NET test framework validates multiple Conductor SDKs through CLI wrappers.

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

## 🔧 CLI Wrappers

All wrappers follow the same architecture pattern:

### Common Structure
```
Wrapper/
├── Main Entry Point          # CLI argument parsing and routing
├── OperationUtils            # Common utilities and error handling
├── Operations/               # Resource-specific operations
│   ├── EventOperations       # Event resource operations
│   └── WorkflowOperations    # Workflow resource operations
└── Extensions/               # Parameter extraction helpers
```

### Standard Interface
All wrappers implement the same CLI interface:
```bash
--operation <operation_name> --parameters <json_parameters> --resource <resource_type>
```

### Response Format
All wrappers return standardized JSON responses:
```json
{
  "success": true,
  "data": { /* operation result */ },
  "errorMessage": null
}
```

## 🔄 Adding New SDK

### Step 1: Create CLI Wrapper
Create a new wrapper following the established pattern:
- **Main entry point**: CLI argument parsing and routing
- **Operation utilities**: Common error handling and response formatting
- **Resource operations**: SDK-specific operation implementations
- **Parameter helpers**: Type-safe parameter extraction

### Step 2: Implement Standard Interface
Ensure the wrapper implements:
- **CLI arguments**: `--operation`, `--parameters`, `--resource`
- **JSON communication**: Standardized input/output format
- **Error handling**: Consistent error reporting
- **Response format**: Standardized success/error responses

### Step 3: Add to Build System
Update build scripts to include the new wrapper:
- **build-wrappers.sh**: Add build option for new SDK
- **run-tests.sh**: Add SDK type parameter
- **CI/CD**: Update GitHub Actions workflows

### Step 4: Test Integration
Verify the new SDK works with existing tests:
- **Environment setup**: Configure `SDK_TYPE` for new SDK
- **Test execution**: Run existing tests against new SDK
- **Response validation**: Ensure responses match API expectations

## 📁 Project Components

### Core Framework
- **SdkTestAutomation.Core**: HTTP client and request resolvers
- **SdkTestAutomation.Api**: Direct API client for comparison
- **SdkTestAutomation.Sdk**: SDK command executor and response comparer
- **SdkTestAutomation.Utils**: Configuration and logging utilities

### CLI Wrappers
- **SdkTestAutomation.CSharp**: C# SDK wrapper
- **SdkTestAutomation.Java**: Java SDK wrapper
- **SdkTestAutomation.Python**: Python SDK wrapper

### Test Framework
- **SdkTestAutomation.Tests**: Test implementations using xUnit v3
- **BaseTest**: Abstract base class with common test utilities
- **Resource APIs**: Direct API clients for each resource type

## 🔧 Configuration

### Environment Variables
```bash
CONDUCTOR_SERVER_URL=http://localhost:8080/api  # Conductor server endpoint
SDK_TYPE=csharp                                  # Target SDK (csharp, java, python)
```

### Test Configuration
Tests use configuration from `SdkTestAutomation.Tests/env.template`:
- **Server settings**: URL, authentication, timeouts
- **Test settings**: Retry policies, validation options
- **Logging**: Output verbosity and format

## 📚 Related Documentation

- **[Main README](README.md)** - Project overview and quick start
- **[Shell Scripts Reference](SCRIPTS_README.md)** - Build and test automation
- **[Adding Operations Guide](ADDING_OPERATIONS_GUIDE.md)** - Adding new operations to wrappers
- **[CLI Wrapper READMEs](SdkTestAutomation.CliWrappers/)** - Language-specific wrapper documentation 