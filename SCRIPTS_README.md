# Shell Scripts Reference

Comprehensive guide to all shell scripts in the SdkTestAutomation project.

## 📁 Scripts Overview

```
scripts/
├── setup-env.sh      # Environment setup and validation
├── build-wrappers.sh # CLI wrapper build management
└── run-tests.sh      # Multi-SDK test execution
```

## 🔧 setup-env.sh

**Purpose**: Sets up and validates the environment configuration for SdkTestAutomation.

### Usage
```bash
./scripts/setup-env.sh [options]
```

### Options

| Option | Description |
|--------|-------------|
| `--minimal` | Quick setup with essential variables only |
| `--full` | Full setup with all configuration options |
| `--validate` | Validate existing `.env` file |
| `--help` | Show help message |

### Examples
```bash
# Quick setup (recommended for most users)
./scripts/setup-env.sh --minimal

# Full setup with all configuration options
./scripts/setup-env.sh --full

# Validate existing environment
./scripts/setup-env.sh --validate
```

### What it does internally
1. **Backup existing .env**: Creates `.env.backup` if `.env` already exists
2. **Copy template**: Copies either `env.example` (minimal) or `env.template` (full) to `.env`
3. **Validate configuration**: Checks for required variables and common issues
4. **Provide guidance**: Shows next steps for manual configuration

### Required Variables Checked
- `CONDUCTOR_SERVER_URL` - Must be set and non-empty
- Additional variables in full mode

## 🔨 build-wrappers.sh

**Purpose**: Builds CLI wrappers for different SDKs (C#, Java, Python) used in testing.

**Why it's needed**: CLI wrappers are language-specific executables that allow .NET tests to communicate with different SDKs through a standardized interface.

### Usage
```bash
./scripts/build-wrappers.sh [options]
```

### Options

| Option | Description |
|--------|-------------|
| `-h, --help` | Show help message |
| `-a, --all` | Build all wrappers (default) |
| `-c, --csharp` | Build only C# wrapper |
| `-j, --java` | Build only Java wrapper |
| `-p, --python` | Build only Python wrapper |
| `-v, --verbose` | Enable verbose output |
| `--clean` | Clean build artifacts before building |

### Examples
```bash
# Build all wrappers
./scripts/build-wrappers.sh

# Build specific wrappers
./scripts/build-wrappers.sh --csharp
./scripts/build-wrappers.sh --java --python

# Clean build (remove existing artifacts)
./scripts/build-wrappers.sh --clean

# Verbose output
./scripts/build-wrappers.sh --verbose
```

### What it does internally

#### C# Wrapper Build
1. **Check .NET SDK**: Verifies .NET 8.0 SDK is installed
2. **Build project**: Runs `dotnet build` on C# CLI wrapper project
3. **Verify output**: Checks that executable is created in `bin/Debug/net8.0/`

#### Java Wrapper Build
1. **Check Java/Maven**: Verifies Java 17+ and Maven are installed
2. **Clean build**: Runs `mvn clean package` to create JAR file
3. **Verify output**: Checks that `sdk-wrapper-1.0.0.jar` is created in `target/`

#### Python Wrapper Build
1. **Check Python**: Verifies Python 3.9+ is installed
2. **Create virtual environment**: Sets up isolated Python environment
3. **Install dependencies**: Installs required packages via `pip install -e .`
4. **Verify setup**: Checks that virtual environment and dependencies are ready

### Build Artifacts Created
- **C#**: `SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp/bin/Debug/net8.0/SdkTestAutomation.CSharp`
- **Java**: `SdkTestAutomation.CliWrappers/SdkTestAutomation.Java/target/sdk-wrapper-1.0.0.jar`
- **Python**: `SdkTestAutomation.CliWrappers/SdkTestAutomation.Python/venv/` (virtual environment)

## 🧪 run-tests.sh

**Purpose**: Executes tests against multiple Conductor SDKs with automatic wrapper building and environment validation.

**Why it's needed**: Provides a unified interface for running tests across different SDKs, handling environment setup, wrapper building, and test execution in a single command.

### Usage
```bash
./scripts/run-tests.sh [sdk_type|options]
```

### Parameters

| Parameter | Description |
|-----------|-------------|
| `csharp` | Run tests with C# SDK only |
| `java` | Run tests with Java SDK only |
| `python` | Run tests with Python SDK only |
| `--help` | Show help message |
| `--validate` | Validate environment and dependencies |

### Examples
```bash
# Run tests with all SDKs
./scripts/run-tests.sh

# Run tests with specific SDK
./scripts/run-tests.sh csharp
./scripts/run-tests.sh java
./scripts/run-tests.sh python

# Validate environment first
./scripts/run-tests.sh --validate
```

### What it does internally

#### Environment Validation
1. **Check .NET SDK**: Verifies .NET 8.0 SDK is available
2. **Check test executable**: Ensures test project is built
3. **Check Java/Maven**: Verifies Java SDK tools (optional)
4. **Check Python**: Verifies Python installation (optional)
5. **Check Conductor server**: Validates server accessibility at `CONDUCTOR_SERVER_URL`

#### Wrapper Setup
1. **Check existing wrappers**: Verifies if CLI wrappers are already built
2. **Auto-build if needed**: Calls `build-wrappers.sh` to build missing wrappers
3. **Handle failures**: Provides clear error messages for build failures

#### Test Execution
1. **Set environment variables**: Configures `SDK_TYPE` and `CONDUCTOR_SERVER_URL`
2. **Run .NET tests**: Executes test project with appropriate filters
3. **Capture results**: Logs test output and exit codes
4. **Provide summary**: Shows which SDKs passed/failed

### Environment Variables Used
- `SDK_TYPE` - Determines which SDK to test (csharp, java, python)
- `CONDUCTOR_SERVER_URL` - Conductor server endpoint (default: http://localhost:8080/api)

### Test Execution Flow
1. **Single SDK**: Runs tests for specified SDK only
2. **All SDKs**: Sequentially runs tests for C#, Java, and Python
3. **Error handling**: Continues with remaining SDKs if one fails
4. **Summary report**: Shows overall success/failure status

## 🔄 Script Dependencies

### Prerequisites
- **setup-env.sh**: No dependencies
- **build-wrappers.sh**: Requires language-specific tools (dotnet, mvn, python)
- **run-tests.sh**: Depends on build-wrappers.sh and setup-env.sh

### Execution Order
```bash
# Typical workflow
./scripts/setup-env.sh --minimal          # 1. Setup environment
./scripts/build-wrappers.sh               # 2. Build wrappers
./scripts/run-tests.sh                    # 3. Run tests

# Or use run-tests.sh which handles steps 2-3 automatically
./scripts/run-tests.sh --validate         # Validate first
./scripts/run-tests.sh                    # Auto-builds and runs tests
```

## 🐛 Troubleshooting

### Common Issues

#### setup-env.sh
- **Permission denied**: Ensure script is executable (`chmod +x scripts/setup-env.sh`)
- **Template not found**: Verify `env.example` and `env.template` exist in `SdkTestAutomation.Tests/`

#### build-wrappers.sh
- **C# build fails**: Check .NET 8.0 SDK installation
- **Java build fails**: Verify Java 17+ and Maven are installed
- **Python build fails**: Ensure Python 3.9+ and pip are available

#### run-tests.sh
- **Conductor server unreachable**: Start server with `docker run -d -p 8080:8080 conductoross/conductor-server:latest`
- **Wrapper not found**: Run `./scripts/build-wrappers.sh` first
- **Test executable missing**: Run `dotnet build SdkTestAutomation.Tests/`

### Debug Mode
```bash
# Enable verbose output
./scripts/build-wrappers.sh --verbose

# Validate environment
./scripts/run-tests.sh --validate

# Check individual components
./scripts/setup-env.sh --validate
```

## 📚 Related Documentation

- **[Main README](../README.md)** - Project overview and quick start
- **[SDK Integration Guide](../SDK_INTEGRATION_GUIDE.md)** - Technical architecture details
- **[CLI Wrapper READMEs](../SdkTestAutomation.CliWrappers/)** - Language-specific wrapper documentation 