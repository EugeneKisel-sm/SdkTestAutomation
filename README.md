# SdkTestAutomation

[![Build and Test](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/evgeniykisel/SdkTestAutomation/actions/workflows/build-and-test.yml)

A .NET test automation framework for API testing, built with xUnit and RestSharp.

## Features

- Built on .NET 9.0
- xUnit test framework integration
- Flexible HTTP request handling with support for multiple content types
- Comprehensive logging system
- Environment configuration management
- Attribute-based request parameter decoration
- Support for JSON, Form URL Encoded, and other content types

## Project Structure

- **SdkTestAutomation.Core**: Core functionality for HTTP requests, attribute handling, and request resolvers
- **SdkTestAutomation.Api**: API-specific implementations and request/response models
- **SdkTestAutomation.Utils**: Utility classes, logging, and configuration management
- **SdkTestAutomation.Tests**: Test implementations and test base classes

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- IDE (Visual Studio, Rider, or VS Code)

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
dotnet test
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

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the terms provided in the LICENSE file.