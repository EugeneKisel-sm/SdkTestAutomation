# Python SDK Integration

This directory contains the Python SDK integration for the SdkTestAutomation framework.

## 🎯 What is the Python SDK Integration?

The Python SDK integration enables testing of Conductor Python SDKs using the SdkTestAutomation framework. It uses Python.NET to bridge between .NET and Python, allowing you to leverage the official Conductor Python SDK while maintaining the unified testing experience.

## 🚀 Why Was It Created?

### The Python Ecosystem Challenge
Python integration presents unique opportunities and challenges:
- **Rich Ecosystem**: Python has excellent data processing and ML libraries
- **Language Bridge**: Need to connect .NET and Python runtimes
- **Dependency Management**: Python uses pip/conda, .NET uses NuGet
- **Runtime Isolation**: Python runs in CPython, .NET in CLR
- **Cross-Platform Support**: Need to work across different operating systems

### The Solution
The Python integration uses Python.NET:
- **Runtime Bridge**: Python.NET provides seamless .NET-to-Python interop
- **Direct SDK Usage**: Uses conductor-python package directly
- **Type Safety**: Leverages Python.NET's type conversion capabilities
- **Performance**: Efficient runtime communication between languages

## 🔧 How It Works

### Architecture Overview

```
.NET Test Framework
    ↓ (Python.NET calls)
Python Runtime (CPython)
    ↓ (HTTP calls via conductor-python)
Conductor Server
```

### Communication Flow

1. **Test Execution**: .NET test calls Python adapter
2. **Python.NET Bridge**: Adapter uses Python.NET to invoke Python code
3. **Python Processing**: Python runtime executes Conductor SDK operations
4. **HTTP Communication**: Python SDK handles HTTP requests to Conductor
5. **Response Conversion**: Python.NET converts Python objects to .NET types

## 📁 Directory Structure

```
Python/
├── PythonClient.cs                  # Main client implementation
├── PythonEventAdapter.cs            # Event operations adapter
├── PythonWorkflowAdapter.cs         # Workflow operations adapter
└── README.md                        # This documentation
```

## 🚀 Getting Started

### Prerequisites

- **Python 3.9+** installed and in PATH
- **conductor-python package** installed
- **Python.NET** (automatically handled by framework)
- **Virtual environment** (recommended)

### 1. Setup Python Environment

```bash
# Create virtual environment (recommended)
python3 -m venv conductor-python-env
source conductor-python-env/bin/activate  # On Windows: conductor-python-env\Scripts\activate

# Install conductor-python
pip install conductor-python
```

### 2. Verify Installation

```bash
# Check Python installation
python3 --version

# Verify conductor-python package
pip list | grep conductor

# Test Python SDK directly
python3 -c "import conductor; print('Python SDK ready')"
```

### 3. Run Tests

```bash
# From project root
SDK_TYPE=python ../../SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests
```

## 📝 How to Use

### Basic Usage

The Python integration works automatically when you set `SDK_TYPE=python`:

```csharp
public class EventTests : BaseConductorTest
{
    [Fact]
    public void AddEvent_ShouldSucceed()
    {
        // This will use Python SDK when SDK_TYPE=python
        var response = EventAdapter.AddEvent("test_event", "test_type", true);
        Assert.True(response.Success);
    }
}
```

### What Happens Behind the Scenes

1. **SDK Selection**: Framework detects `SDK_TYPE=python`
2. **Adapter Creation**: Creates `PythonEventAdapter` instance
3. **Python.NET Call**: Adapter uses Python.NET to invoke Python code
4. **Python Execution**: Python runtime executes Conductor operations
5. **Response Conversion**: Python objects converted to .NET types

## 📊 Request/Response Format

### Python.NET Integration

The Python integration uses Python.NET for seamless interop:

```csharp
// Python.NET usage in adapters
dynamic conductor = Py.Import("conductor.client.http.api.event_api");
dynamic eventApi = conductor.EventApi(conductorClient);
dynamic result = eventApi.add_event_handler(eventHandler);
```

### Response Format

All operations return the framework's `SdkResponse` format:

```csharp
public class SdkResponse
{
    public bool Success { get; set; }
    public string Content { get; set; }
    public string ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}
```

## 🔧 Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `CONDUCTOR_SERVER_URL` | Conductor server URL | `http://localhost:8080/api` |
| `PYTHONPATH` | Python module search path | Auto-detected |

### Python Environment

The integration automatically configures the Python environment:

```csharp
// Python.NET initialization
PythonEngine.Initialize();
using (Py.GIL()) // Global Interpreter Lock
{
    // Python code execution
    dynamic conductor = Py.Import("conductor.client.http.api.event_api");
}
```

## 🎯 Use Cases

### For Data Scientists
- Test Conductor workflows in data processing pipelines
- Validate ML model deployment workflows
- Integrate with Python data science libraries
- Leverage Python's rich ecosystem

### For Python Developers
- Test Python applications with Conductor integration
- Validate conductor-python SDK behavior
- Ensure compatibility with different Conductor versions
- Debug Python SDK integration issues

### For Cross-Language Teams
- Use Python for data processing workflows
- Integrate Python services with .NET applications
- Validate API consistency across languages
- Leverage Python's strengths in data handling

## 🔧 Troubleshooting

### Common Issues

**Python Not Found:**
```bash
# Check Python installation
python3 --version

# Verify Python is in PATH
which python3
```

**conductor-python Not Installed:**
```bash
# Install conductor-python
pip install conductor-python

# Verify installation
pip list | grep conductor
```

**Python.NET Issues:**
```bash
# Check Python.NET package
dotnet list package | grep Python

# Restore packages
dotnet restore
```

**Virtual Environment Issues:**
```bash
# Activate virtual environment
source conductor-python-env/bin/activate

# Install packages in virtual environment
pip install conductor-python
```

### Debug Mode

Enable debug logging to see detailed Python.NET communication:

```csharp
// In your test setup
_logger.LogLevel = "Debug";
```

This will show:
- Python.NET initialization
- Python method calls
- Type conversions
- Error details

## 🔄 Extending Python Integration

### Adding New Operations

1. **Add Python Method**: Use conductor-python SDK method
2. **Update Adapter**: Add corresponding method in adapter class
3. **Add Tests**: Create tests for the new operation

## 📊 Performance Considerations

### Advantages
- **Rich Ecosystem**: Access to Python's extensive library ecosystem
- **Data Processing**: Excellent for data science and ML workflows
- **Type Conversion**: Python.NET provides efficient type conversion
- **Runtime Integration**: Seamless .NET-to-Python communication
- **Cross-Platform**: Works on Windows, macOS, and Linux

### Trade-offs
- **Runtime Overhead**: Python interpreter startup and GIL management
- **Memory Usage**: Additional Python runtime memory
- **Type Safety**: Dynamic typing vs static typing
- **Performance**: Slower than native .NET or Go integrations

### Performance Comparison

| Aspect | Python Integration | Other Integrations |
|--------|-------------------|-------------------|
| **Startup Time** | Python interpreter startup | Instant (C#) / Process startup (Java) |
| **Memory Usage** | Python runtime + .NET | Minimal (C#) / Process memory (Java) |
| **Type Safety** | Dynamic typing | Static typing (C#) / Compile-time (Java) |
| **Ecosystem Access** | Full Python ecosystem | Limited to language ecosystem |
| **Data Processing** | Excellent | Good (C#) / Limited (Java) |

### Optimization Tips
- **GIL Management**: Use `using (Py.GIL())` for thread safety
- **Object Reuse**: Reuse Python objects when possible
- **Error Handling**: Implement proper exception handling
- **Resource Cleanup**: Ensure proper disposal of Python objects

## 🔗 Related Documentation

- [Main Framework README](../../../README.md) - Overview of the entire framework
- [Conductor Python SDK](https://github.com/Netflix/conductor/tree/main/client/python) - Official Python SDK
- [conductor-python PyPI](https://pypi.org/project/conductor-python/) - Python package information
- [Python.NET Documentation](https://pythonnet.github.io/) - Python.NET interop library
- [Python Virtual Environments](https://docs.python.org/3/tutorial/venv.html) - Python environment management 