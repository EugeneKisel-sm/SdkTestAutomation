# SDK Integration Guide

## 📋 Overview

| SDK | Status | Integration Method | Package/Bridge |
|-----|--------|-------------------|----------------|
| **C#** | ✅ Ready | Direct NuGet | `conductor-csharp` v1.1.3 |
| **Java** | ✅ Ready | IKVM.NET Bridge | `conductor-client.jar` + `conductor-common.jar` |
| **Python** | ✅ Ready | Python.NET Bridge | `conductor-python` |
| **Go** | ✅ Ready | Process Communication | `conductor-go` |

## 🚀 Quick Start

```bash
# Run automated setup
./setup-sdks.sh

# Test each SDK
TEST_SDK=csharp dotnet test SdkTestAutomation.Tests
TEST_SDK=java dotnet test SdkTestAutomation.Tests
TEST_SDK=python dotnet test SdkTestAutomation.Tests
TEST_SDK=go dotnet test SdkTestAutomation.Tests
```

## 🔧 Detailed Setup

### C# SDK
- **Package**: `conductor-csharp` v1.1.3
- **Method**: Direct NuGet package reference
- **No additional setup required**

### Java SDK
- **Prerequisites**: Java 17+, IKVM.NET v8.12.0
- **JAR Files**: `conductor-client.jar`, `conductor-common.jar` in `lib/`
- **Method**: IKVM.NET bridge converts JARs to .NET assemblies

### Python SDK
- **Prerequisites**: Python 3.9+, pip
- **Package**: `conductor-python` from PyPI
- **Method**: Python.NET bridge for .NET ↔ Python communication

### Go SDK
- **Prerequisites**: Go 1.19+, go modules
- **Package**: `conductor-go` from GitHub
- **Method**: HTTP API Bridge for .NET ↔ Go communication
- **Dependencies**: `gorilla/mux` for HTTP routing

## 🧪 Testing

```bash
# Test individual SDKs
TEST_SDK=csharp dotnet test SdkTestAutomation.Tests
TEST_SDK=java dotnet test SdkTestAutomation.Tests
TEST_SDK=python dotnet test SdkTestAutomation.Tests
TEST_SDK=go dotnet test SdkTestAutomation.Tests

# Test all SDKs
for sdk in csharp java python go; do
    TEST_SDK=$sdk dotnet test SdkTestAutomation.Tests
done
```

## 🔍 Troubleshooting

### Common Issues

- **C# SDK**: Package not found → Run `dotnet restore`
- **Java SDK**: JAR files missing → Download from Conductor releases
- **Python SDK**: ModuleNotFoundError → Install `conductor-python` package
- **Go SDK**: Module not found → Run `go get github.com/conductor-sdk/conductor-go`

### Debug Mode

```csharp
_logger.LogLevel = "Debug";
```

## 📦 Package Sources

- **C# SDK**: [conductor-csharp](https://github.com/Netflix/conductor-csharp)
- **Java SDK**: [conductor-oss/java-sdk](https://github.com/conductor-oss/java-sdk)
- **Python SDK**: [conductor-oss/python-sdk](https://github.com/conductor-oss/python-sdk)
- **Go SDK**: [conductor-oss/go-sdk](https://github.com/conductor-oss/go-sdk)

## 🔄 Version Compatibility

| Component | C# SDK | Java SDK | Python SDK | Go SDK |
|-----------|--------|----------|------------|--------|
| **Conductor Server** | 1.x | 4.x | 4.x | 4.x |
| **SDK Version** | 1.1.3 | 4.0.12 | Latest | Latest |
| **Runtime** | .NET 8.0 | Java 17+ | Python 3.9+ | Go 1.19+ |
| **Bridge** | Direct | IKVM.NET | Python.NET | HTTP API |
| **Repository** | [conductor-csharp](https://github.com/Netflix/conductor-csharp) | [conductor-oss/java-sdk](https://github.com/conductor-oss/java-sdk) | [conductor-oss/python-sdk](https://github.com/conductor-oss/python-sdk) | [conductor-oss/go-sdk](https://github.com/conductor-oss/go-sdk) | 