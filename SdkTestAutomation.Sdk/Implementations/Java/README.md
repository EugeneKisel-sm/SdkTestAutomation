# Java SDK Integration - CLI Approach

This directory contains the Java SDK integration using a CLI-based approach instead of IKVM.NET.

## Overview

The Java SDK integration has been updated to use a CLI-based approach that:

1. **Eliminates IKVM.NET dependency** - No longer requires IKVM.NET which doesn't support Java 17+
2. **Uses Java 17+** - Supports modern Java versions
3. **Process-based communication** - C# code invokes Java CLI applications via Process.Start
4. **JSON-based communication** - Requests and responses are exchanged via JSON
5. **Separated CLI applications** - Separate JAR files for Conductor and Orkes operations

## Architecture

```
C# Application
    ↓ (Process.Start)
Java CLI Applications
    ├── conductor-java-cli.jar (Conductor operations)
    └── orkes-java-cli.jar (Orkes operations)
    ↓ (HTTP calls)
Conductor/Orkes Server
```

## Components

### Java CLI Applications

- **ConductorCli.java** - Main entry point for Conductor operations (events, workflows)
- **OrkesCli.java** - Main entry point for Orkes-specific operations (tokens)

### Operation Classes

The operations are organized into separate packages:

#### Conductor Operations (`operations/conductor/`)
- **EventOperations.java** - Handles Conductor event operations
- **WorkflowOperations.java** - Handles Conductor workflow operations

#### Orkes Operations (`operations/orkes/`)
- **TokenOperations.java** - Handles Orkes token operations

### Shared Components

- **SdkResponse.java** - Response model for all operations
- **OperationUtils.java** - Utility functions for error handling and client configuration

### C# Adapters

- **JavaClient.cs** - Base client for Java CLI communication
- **JavaEventAdapter.cs** - Event operations adapter
- **JavaWorkflowAdapter.cs** - Workflow operations adapter
- **JavaTokenAdapter.cs** - Token operations adapter (Orkes)

## Setup

### Prerequisites

1. **Java 17+** installed and in PATH
2. **Maven** installed for building Java CLI applications

### Building Java CLI Applications

```bash
cd SdkTestAutomation.Sdk/Implementations/Java/cli-java-sdk
chmod +x build.sh
./build.sh
```

This will:
- Build both Java CLI applications using Maven
- Create separate shaded JAR files for Conductor and Orkes
- Copy JAR files to the appropriate output directory

### JAR File Locations

The build script copies JAR files to:
- `SdkTestAutomation.Sdk/bin/Debug/net8.0/lib/conductor-client.jar`
- `SdkTestAutomation.Sdk/bin/Debug/net8.0/lib/orkes-conductor-client.jar`

## Communication Protocol

### Request Format

```json
{
  "method": "addEvent",
  "serverUrl": "http://localhost:8080/api",
  "data": {
    "name": "test_event",
    "eventType": "test_event_type",
    "active": true
  }
}
```

### Response Format

```json
{
  "success": true,
  "data": "Event added successfully"
}
```

### Error Response

```json
{
  "success": false,
  "error": "Error message"
}
```

## Supported Operations

### Conductor Operations (conductor-java-cli.jar)

- `add-event` - Register a new event handler
- `get-event` - Get all event handlers
- `get-event-by-name` - Get event handlers by name
- `update-event` - Update an existing event handler
- `delete-event` - Delete an event handler
- `get-workflow` - Get workflow execution status
- `get-workflows` - Get running workflows
- `start-workflow` - Start a new workflow
- `terminate-workflow` - Terminate a workflow

### Orkes Operations (orkes-java-cli.jar)

- `generate-token` - Generate authentication token
- `get-user-info` - Get user information (not implemented)

## CLI Usage

### Conductor CLI

```bash
java -jar conductor-client.jar --resource event --operation get-event --parameters '{}'
java -jar conductor-client.jar --resource workflow --operation get-workflow --parameters '{"workflowId":"123"}'
```

### Orkes CLI

```bash
java -jar orkes-conductor-client.jar --resource token --operation generate-token --parameters '{"keyId":"test","keySecret":"test"}'
```

## Troubleshooting

### Common Issues

1. **Java not found**
   - Ensure Java 17+ is installed and in PATH
   - Run `java -version` to verify

2. **JAR files not found**
   - Run the build script: `./build.sh`
   - Check JAR files exist in the expected locations

3. **Maven not found**
   - Install Maven: `brew install maven` (macOS) or download from maven.apache.org

4. **Permission denied**
   - Make build script executable: `chmod +x build.sh`

### Debug Mode

Enable debug logging in the C# application to see detailed communication:

```csharp
_logger.LogLevel = "Debug";
```

## Migration from IKVM.NET

The migration from IKVM.NET to CLI approach involved:

1. **Removed IKVM dependencies** from project file
2. **Replaced dynamic Java calls** with Process.Start invocations
3. **Created separate Java CLI applications** for Conductor and Orkes operations
4. **Organized operations** into conductor and orkes packages
5. **Updated C# adapters** to use JSON communication
6. **Added build scripts** for Java CLI compilation

## Performance Considerations

- **Process startup overhead** - Each Java call starts a new process
- **JSON serialization** - Additional overhead for request/response serialization
- **Memory usage** - JVM startup for each operation

For high-performance scenarios, consider:
- Process pooling
- Batch operations
- Caching strategies 