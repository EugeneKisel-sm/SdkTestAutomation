# Java SDK Integration

## Prerequisites

- Java 17+
- IKVM.NET v8.12.0 (configured in project)

## Setup

Verify JAR files in `lib/` directory:
- `conductor-client.jar` (65KB)
- `conductor-common.jar` (104KB)

## Testing

```bash
TEST_SDK=java dotnet test SdkTestAutomation.Tests
```

## JAR Sources

- [Official Conductor releases](https://github.com/conductor-oss/conductor/releases)
- [Java SDK Repository](https://github.com/conductor-oss/java-sdk)

## Version Compatibility

- **Conductor Server**: 3.x
- **Java SDK**: 3.15.0 