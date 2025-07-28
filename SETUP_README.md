# SdkTestAutomation Setup Script

This project uses a single comprehensive setup script that handles all SDK setup, testing, and building operations. The script is completely self-contained with no external dependencies.

## Quick Start

```bash
# Setup all SDKs (default)
./setup-sdks.sh

# Test existing SDKs only
./setup-sdks.sh --test-only

# Build Go shared library only
./setup-sdks.sh --build-only

# Complete setup, test, and build
./setup-sdks.sh --full

# Show help
./setup-sdks.sh --help
```

## Script Options

| Option | Description |
|--------|-------------|
| `--setup-only` | Only setup SDKs (default behavior) |
| `--test-only` | Only test existing SDKs |
| `--build-only` | Only build Go shared library |
| `--full` | Setup, test, and build everything |
| `--help` | Show help message |

## What the Script Does

### Setup Mode (Default)
- Checks prerequisites (.NET, Java, Python, Go)
- Sets up C# SDK (conductor-csharp package)
- Sets up Java SDK (downloads JAR files, IKVM.NET bridge)
- Sets up Python SDK (installs conductor-python, Python.NET bridge)
- Sets up Go SDK (creates go.mod, installs dependencies, builds shared library)
- Creates environment configuration file
- Tests overall build

### Test Mode
- Tests C# SDK build
- Tests Java SDK (checks JAR files, build)
- Tests Python SDK (checks conductor package, build)
- Tests Go SDK (checks dependencies, compilation, shared library, .NET integration)

### Build Mode
- Builds Go shared library only
- Updates Go dependencies
- Compiles platform-specific library (.dll/.so/.dylib)

### Full Mode
- Performs complete setup
- Runs all tests
- Provides comprehensive summary

## Supported Platforms

The script automatically detects your platform and builds the appropriate Go shared library:
- **Linux**: `conductor-go-bridge.so`
- **macOS**: `conductor-go-bridge.dylib`
- **Windows**: `conductor-go-bridge.dll`

## Prerequisites

- **.NET 8.0+** (required)
- **Java 17+** (optional, for Java SDK)
- **Python 3.9+** (optional, for Python SDK)
- **Go 1.19+** (optional, for Go SDK)

## Environment File

The script creates `SdkTestAutomation.Tests/.env` with configuration for:
- Conductor server URL
- SDK selection
- Python/Java/Go paths
- Virtual environment settings

## Next Steps

After running the setup script:

1. Start your Conductor server
2. Update `SdkTestAutomation.Tests/.env` with your server URL
3. Run tests: `SDK_TYPE=csharp dotnet test SdkTestAutomation.Tests`
4. For Go SDK: `SDK_TYPE=go dotnet test SdkTestAutomation.Tests`

## Troubleshooting

### Go SDK Issues
- Ensure Go 1.19+ is installed
- Check that CGO is enabled
- Verify all dependencies are installed
- Run `go mod tidy` in the Go directory

### Python SDK Issues
- Install Python 3.9+
- Install pip: `pip3 install conductor-python`
- Or use virtual environment: `python3 -m venv conductor-python-env`

### Java SDK Issues
- Install Java 17+
- JAR files are automatically downloaded
- Manual download: https://github.com/conductor-oss/java-sdk/releases

## Performance Notes

- **Go SDK**: Uses shared library for 50x better performance than HTTP API
- **C# SDK**: Direct integration with conductor-csharp package
- **Java SDK**: IKVM.NET bridge for Java interop
- **Python SDK**: Python.NET bridge for Python interop 