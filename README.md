# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)
[![Build](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build.yml)

A .NET test automation framework for API testing, built with xUnit and RestSharp.

## 🚀 CI/CD Status

### Workflows
- **Build and Test**: Runs on pull requests and manual triggers
  - Builds the solution
  - Deploys Conductor server in Docker
  - Executes all tests
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

## Features

- Built on .NET 8.0
- xUnit test framework integration
- Flexible HTTP request handling with support for multiple content types
- Comprehensive logging system
- Environment configuration management
- Attribute-based request parameter decoration
- Support for JSON, Form URL Encoded, and other content types
- Automated CI/CD with comprehensive test reporting

## Project Structure

- **SdkTestAutomation.Core**: Core functionality for HTTP requests, attribute handling, and request resolvers
- **SdkTestAutomation.Api**: API-specific implementations and request/response models
- **SdkTestAutomation.Utils**: Utility classes, logging, and configuration management
- **SdkTestAutomation.Tests**: Test implementations and test base classes

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- IDE (Visual Studio, Rider, or VS Code)
- Docker (for running Conductor server locally)

### Configuration

1. Clone the repository
2. Rename `.env.template` to `.env` in the `SdkTestAutomation.Tests` directory
3. Update the `.env` file with your configuration:

```env
CONDUCTOR_SERVER_URL=your_api_url
CONDUCTOR_AUTH_KEY=your_auth_key
CONDUCTOR_AUTH_SECRET=your_auth_secret
```

### Running Tests

```bash
./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## Writing Tests

### Base Test Class

All test classes should inherit from `BaseTest` which provides:
- Automatic test logging
- Test context management
- API client initialization

Example:
```csharp
public class MyTests : BaseTest
{
    [Fact]
    public void MyTest()
    {
        // Your test implementation
    }
}
```

### Request Models

Create request models by inheriting from `HttpRequest` and using attributes:

```csharp
public class MyRequest : HttpRequest
{
    [UrlParameter]
    public string QueryParam { get; set; }

    [Header(Name = "Custom-Header")]
    public string HeaderValue { get; set; }

    [Body]
    public string RequestBody { get; set; }
}
```

### Available Attributes

- `[UrlParameter]`: Add query parameters to the URL
- `[Header]`: Add HTTP headers
- `[Body]`: Specify request body content
- All attributes support custom naming via the `Name` property

### Supported Content Types

- JSON (`ContentType.Json`)
- Form URL Encoded (`ContentType.FormUrlEncoded`)
- None (`ContentType.None`)

## Logging

The framework includes a built-in logging system that automatically captures:
- Request details (URL, headers, body)
- Response information (status code, body)
- Test execution timestamps
- Custom log messages

## Test Reports

### Viewing Test Results

1. **GitHub Actions**: Go to the Actions tab and click on a workflow run
2. **Test Results Tab**: View TRX and JUnit results directly in GitHub
3. **Artifacts**: Download the `test-results` artifact for all report formats
4. **Job Summary**: View a formatted test summary in the workflow job summary

### Report Formats

- **HTML**: Interactive report with expandable test details
- **TRX**: Native .NET format, viewable in Visual Studio
- **JUnit**: Standard XML format for CI/CD tools

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

### Development Workflow

1. Make your changes
2. Ensure all tests pass locally
3. Push your changes (triggers build workflow)
4. Create a PR (triggers full build and test workflow)
5. Check test results in the PR checks

## License

This project is licensed under the terms provided in the LICENSE file.