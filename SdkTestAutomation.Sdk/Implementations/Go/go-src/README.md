# Go SDK Bridge Source Code

This folder contains the Go source code for the Conductor Go SDK bridge that enables .NET applications to interact with the Conductor Go SDK through a shared library interface.

## Files

- `conductor-go-bridge.go` - Main Go source file containing CGO exports for .NET interop
- `go.mod` - Go module definition and dependencies
- `go.sum` - Go module checksums
- `README.md` - This documentation file
- `build.sh` - Cross-platform build script

## Building

To build the shared library for your platform:

### Using the Build Script (Recommended)
```bash
./build.sh
```

This will automatically:
- Detect your platform
- Create the `../build-artifacts/` directory
- Build the appropriate shared library
- Output to `../build-artifacts/conductor-go-bridge.{dylib|so|dll}`

### Manual Building

#### macOS (dylib)
```bash
mkdir -p ../build-artifacts
go build -buildmode=c-shared -o ../build-artifacts/conductor-go-bridge.dylib conductor-go-bridge.go
```

#### Linux (so)
```bash
mkdir -p ../build-artifacts
go build -buildmode=c-shared -o ../build-artifacts/conductor-go-bridge.so conductor-go-bridge.go
```

#### Windows (dll)
```bash
mkdir -p ../build-artifacts
go build -buildmode=c-shared -o ../build-artifacts/conductor-go-bridge.dll conductor-go-bridge.go
```

## Dependencies

The project depends on:
- `github.com/conductor-sdk/conductor-go` - Official Conductor Go SDK
- `github.com/gorilla/mux` - HTTP router (if using HTTP bridge approach)

## Architecture

This bridge provides CGO exports that allow .NET applications to:
- Create and manage Conductor API clients
- Add, update, delete, and retrieve events
- Start, get, and terminate workflows
- Capture and retrieve logs

The bridge maintains client state and provides thread-safe operations for concurrent .NET usage.

## Build Artifacts

Compiled shared libraries and generated files are output to the `../build-artifacts/` directory:
- `conductor-go-bridge.dylib` - macOS shared library
- `conductor-go-bridge.so` - Linux shared library
- `conductor-go-bridge.dll` - Windows shared library
- `conductor-go-bridge.h` - Generated C header file
- `conductor-go-bridge` - Compiled executable

These files are automatically copied to the .NET output directory during build and used by the Go SDK adapters (`GoClient`, `GoEventAdapter`, `GoWorkflowAdapter`).

## File Organization

- **Source files** (this directory): Go source code, module files, build script
- **Build artifacts** (`../build-artifacts/`): Compiled libraries and generated files
- **C# adapters** (`../`): .NET adapter implementations