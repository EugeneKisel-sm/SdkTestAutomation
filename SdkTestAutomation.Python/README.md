# Python SDK Integration

## Prerequisites

- Python 3.9+
- pip package manager

## Setup

```bash
# Install Conductor Python SDK
pip install conductor-python

# Verify installation
python -c "import conductor; print('SDK installed successfully')"
```

## Environment Variables

```env
PYTHON_HOME=/path/to/your/python
PYTHONPATH=/path/to/site-packages
PYTHON_VENV_PATH=/path/to/conductor-python-env  # If using virtual environment
```

## Testing

```bash
TEST_SDK=python dotnet test SdkTestAutomation.Tests
```

## Virtual Environment

If setup script created virtual environment:

```bash
source conductor-python-env/bin/activate
TEST_SDK=python dotnet test SdkTestAutomation.Tests
```

## Installation Methods

For externally managed environments:

```bash
pip install --user conductor-python
# or
python3 -m venv conductor-env && source conductor-env/bin/activate && pip install conductor-python
``` 