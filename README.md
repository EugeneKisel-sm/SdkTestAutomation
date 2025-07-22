# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A .NET test automation framework for validating multiple Conductor SDKs (C#, Java, Python) through CLI wrappers. Built with xUnit v3, RestSharp, and a language-agnostic architecture.

## 🎯 Key Features

- **Multi-SDK Support**: Test C#, Java, and Python Conductor SDKs with a single test codebase
- **CLI-First Architecture**: Language-agnostic approach using command-line wrappers
- **Deep Validation**: Compare SDK responses with direct REST API calls for structural equality
- **Extensible Design**: Easy to add new SDKs with minimal changes
- **Comprehensive Logging**: Detailed execution logs and error reporting
- **CI/CD Ready**: Automated testing with comprehensive reporting

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

## 🚀 CI/CD Status

### Workflows
- **Build and Test**: Runs on pull requests and manual triggers
  - Builds the solution and CLI wrappers
  - Deploys Conductor server in Docker
  - Executes tests against all supported SDKs
  - Generates comprehensive test reports (HTML, TRX, JUnit)
  - Publishes test results to GitHub Checks

- **Build**: Runs on every push
  - Quick build validation
  - Ensures code compiles correctly

### Test Results
Test results are automatically generated and available in multiple formats:
- **HTML Report**: Rich interactive report with detailed test information
- **TRX Report**: Native .NET test results format
- **JUnit XML**: Standard XML format for CI/CD integration

All reports are uploaded as artifacts and can be downloaded from the workflow run page.

## 📁 Project Structure

```
SdkTestAutomation/
├── SdkTestAutomation.Common/           # Shared models and helpers
│   ├── Models/
│   │   ├── SdkCommand.cs              # Standardized command structure
│   │   └── SdkResponse.cs             # Standardized response structure
│   └── Helpers/
│       ├── SdkCommandExecutor.cs      # CLI wrapper execution engine
│       └── ResponseComparer.cs        # Response validation logic
├── SdkTestAutomation.CliWrappers/      # CLI wrappers for each SDK
│   ├── SdkTestAutomation.CSharp/       # C# SDK wrapper
│   ├── SdkTestAutomation.Java/         # Java SDK wrapper
│   └── SdkTestAutomation.Python/       # Python SDK wrapper
├── SdkTestAutomation.Tests/            # Test implementations
├── SdkTestAutomation.Api/              # Direct API client
├── SdkTestAutomation.Core/             # Core HTTP functionality
├── SdkTestAutomation.Utils/            # Utilities and logging
└── run-tests.sh                        # Multi-SDK test runner
```

## 🛠️ Getting Started

### Prerequisites

- .NET 8.0 SDK
- Java 17+ and Maven (for Java SDK testing)
- Python 3.9+ and pip (for Python SDK testing)
- Docker (for running Conductor server locally)
- IDE (Visual Studio, Rider, or VS Code)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/evgeniykisel/SdkTestAutomation.git
   cd SdkTestAutomation
   ```

2. **Start Conductor server**
   ```bash
   docker run -d -p 8080:8080 conductoross/conductor-server:latest
   ```

3. **Run tests with all SDKs**
   ```bash
   CONDUCTOR_SERVER_URL=http://localhost:8080/api ./run-tests.sh
   ```

4. **Run tests with specific SDK**
   ```bash
   CONDUCTOR_SERVER_URL=http://localhost:8080/api ./run-tests.sh csharp
   CONDUCTOR_SERVER_URL=http://localhost:8080/api ./run-tests.sh java
   CONDUCTOR_SERVER_URL=http://localhost:8080/api ./run-tests.sh python
   ```

### Configuration

Set the required environment variables:

```bash
export CONDUCTOR_SERVER_URL=http://localhost:8080/api
export SDK_TYPE=csharp  # or java, python
```

## 🧪 Writing Tests

### Base Test Class

All test classes inherit from `BaseTest` which provides SDK integration:

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

### SDK Operations

The framework supports these standardized operations:

- `add-event`: Create a new event handler
- `get-event`: Retrieve all event handlers
- `get-event-by-name`: Get event handlers by name
- `update-event`: Update an existing event handler
- `delete-event`: Delete an event handler
- `get-workflow`: Retrieve workflow execution status

### Response Validation

The framework automatically validates that SDK responses match direct API responses:

```csharp
// This validates structural equality between SDK and API responses
var isValid = await ValidateSdkResponseAsync<GetEventResponse>(sdkResponse, apiResponse);
Assert.True(isValid, "SDK response does not match API response");
```

## 🔧 CLI Wrappers

### C# Wrapper

**Build:**
```bash
dotnet build SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/SdkTestAutomation.CSharp.csproj
```

**Test:**
```bash
dotnet run --project SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp -- \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

### Java Wrapper

**Build:**
```bash
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Java
mvn clean package
```

**Test:**
```bash
java -jar SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

### Python Wrapper

**Install:**
```bash
cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Python
pip install -e .
```

**Test:**
```bash
python SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/sdk_wrapper/main.py \
  --operation add-event \
  --parameters '{"name":"test","event":"test_event","active":true}' \
  --resource event
```

## 📊 Test Reports

### Viewing Test Results

1. **GitHub Actions**: Go to the Actions tab and click on a workflow run
2. **Test Results Tab**: View TRX and JUnit results directly in GitHub
3. **Artifacts**: Download the `test-results` artifact for all report formats
4. **Job Summary**: View a formatted test summary in the workflow job summary

### Report Formats

- **HTML**: Interactive report with expandable test details
- **TRX**: Native .NET format, viewable in Visual Studio
- **JUnit**: Standard XML format for CI/CD tools

## 🔄 Adding a New SDK

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
       return _sdkType switch
       {
           "csharp" => (...),
           "java" => (...),
           "python" => (...),
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

## 🐛 Troubleshooting

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

Enable detailed logging:

```bash
LOG_LEVEL=Debug SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

### Development Workflow

1. Make your changes
2. Ensure all tests pass locally for all SDKs
3. Push your changes (triggers build workflow)
4. Create a PR (triggers full build and test workflow)
5. Check test results in the PR checks

## 📄 License

This project is licensed under the terms provided in the LICENSE file.