# Go Implementation Structure

## Overview
This directory contains the Go SDK implementation for the SdkTestAutomation framework, organized to separate source code from build artifacts.

## Directory Structure

```
Go/
├── go-src/                          # Go source code directory
│   ├── conductor-go-bridge.go       # Main Go source file with CGO exports
│   ├── conductor-go-bridge.h        # C header file for .NET interop
│   ├── go.mod                       # Go module definition
│   ├── go.sum                       # Go module checksums
│   ├── README.md                    # Go source documentation
│   └── build.sh                     # Cross-platform build script
├── build-artifacts/                 # Build artifacts directory (IGNORED)
│   ├── conductor-go-bridge.dylib    # Compiled shared library (macOS)
│   ├── conductor-go-bridge.so       # Compiled shared library (Linux)
│   ├── conductor-go-bridge.dll      # Compiled shared library (Windows)
│   └── conductor-go-bridge          # Compiled executable
├── GoClient.cs                      # .NET client for Go shared library
├── GoEventAdapter.cs                # .NET event adapter implementation
├── GoWorkflowAdapter.cs             # .NET workflow adapter implementation
├── GO_SDK_INTEGRATION_APPROACHES.md # Integration approaches documentation
└── STRUCTURE.md                     # This documentation file
```

## File Categories

### Source Files (Tracked in Git)
- **Go Source**: `go-src/conductor-go-bridge.go` - Main Go implementation
- **Module Files**: `go-src/go.mod`, `go-src/go.sum` - Go module management
- **Header Files**: `go-src/conductor-go-bridge.h` - C header for .NET interop
- **Documentation**: `go-src/README.md` - Go source documentation
- **Build Scripts**: `go-src/build.sh` - Cross-platform build script
- **C# Adapters**: `*.cs` files - .NET adapter implementations
- **Documentation**: `GO_SDK_INTEGRATION_APPROACHES.md` - Integration guide

### Build Artifacts (Ignored by Git)
- `build-artifacts/conductor-go-bridge.dylib` - macOS shared library
- `build-artifacts/conductor-go-bridge.so` - Linux shared library  
- `build-artifacts/conductor-go-bridge.dll` - Windows shared library
- `build-artifacts/conductor-go-bridge` - Compiled executable

## Gitignore Configuration

The `.gitignore` file has been updated to:
- Ignore the entire `build-artifacts/` directory and its contents
- Track source files in the `go-src/` directory
- Ignore Go build cache files (`*.test`, `*.out`, `go.work`)

## Building

To build the Go shared library:
```bash
cd SdkTestAutomation.Sdk/Implementations/Go/go-src
./build.sh
```

This will generate the appropriate shared library in the `../build-artifacts/` directory.

## Architecture

The Go implementation provides:
- CGO exports for .NET interop
- Conductor API client management
- Event and workflow operations
- Thread-safe operations for concurrent .NET usage
- Logging and error handling

## Build Process

1. **Source Code**: Located in `go-src/` directory
2. **Build Script**: `go-src/build.sh` handles cross-platform compilation
3. **Output**: Compiled artifacts go to `build-artifacts/` directory
4. **Integration**: .NET project references artifacts from `build-artifacts/`
5. **Deployment**: Build artifacts are copied to .NET output directory during build