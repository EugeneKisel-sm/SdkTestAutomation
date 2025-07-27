#!/bin/bash

# SDK Setup Script for SdkTestAutomation
# This script helps set up all three Conductor SDKs

set -e

echo "🚀 SdkTestAutomation SDK Setup Script"
echo "======================================"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Version check functions
check_version() {
    local version=$1
    local required=$2
    if [ "$(printf '%s\n' "$required" "$version" | sort -V | head -n1)" = "$required" ]; then 
        return 0
    else
        return 1
    fi
}

# Check if .NET is installed
check_dotnet() {
    print_status "Checking .NET installation..."
    if command -v dotnet &> /dev/null; then
        local version=$(dotnet --version)
        print_success ".NET $version is installed"
    else
        print_error ".NET is not installed. Please install .NET 8.0 or later."
        exit 1
    fi
}

# Setup C# SDK
setup_csharp_sdk() {
    print_status "Setting up C# SDK..."
    
    # C# SDK is integrated directly in SdkTestAutomation.Sdk project
    # Just verify the conductor-csharp package is available
    print_status "Verifying conductor-csharp package..."
    
    # Build the main SDK project to verify C# integration
    dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    dotnet build SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    
    print_success "C# SDK setup complete (conductor-csharp v1.1.3)"
}

# Setup Java SDK
setup_java_sdk() {
    print_status "Setting up Java SDK..."
    
    # Check if Java is installed with version check
    if command -v java &> /dev/null; then
        local version=$(java -version 2>&1 | head -n 1 | cut -d'"' -f2)
        if check_version "$version" "17.0.0"; then
            print_success "Java $version is installed (meets minimum requirement of 17)"
        else
            print_warning "Java $version is installed but version 17+ is required"
            return 1
        fi
    else
        print_warning "Java is not installed. Please install Java 17+ for Java SDK support."
        return 1
    fi
    
    # Check JAR files
    local jar_dir="SdkTestAutomation.Sdk/lib"
    if [ -f "$jar_dir/conductor-client.jar" ] && [ -f "$jar_dir/conductor-common.jar" ]; then
        print_success "JAR files found in $jar_dir"
    else
        print_warning "JAR files not found in $jar_dir"
        print_status "Downloading latest conductor v4.x JAR files..."
        mkdir -p "$jar_dir"
        curl -L -o "$jar_dir/conductor-client.jar" "https://repo1.maven.org/maven2/com/netflix/conductor/conductor-client/4.0.12/conductor-client-4.0.12.jar"
        curl -L -o "$jar_dir/conductor-common.jar" "https://repo1.maven.org/maven2/com/netflix/conductor/conductor-common/4.0.12/conductor-common-4.0.12.jar"
        if [ $? -eq 0 ]; then
            print_success "JAR files downloaded successfully"
        else
            print_warning "Failed to download JAR files automatically"
            print_status "Please download manually from https://github.com/conductor-oss/java-sdk/releases"
            return 1
        fi
    fi
    
    # Build Java project
    dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    dotnet build SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    
    print_success "Java SDK setup complete (IKVM.NET bridge)"
}

# Setup Python SDK
setup_python_sdk() {
    print_status "Setting up Python SDK..."
    
    # Check if Python is installed with version check
    if command -v python3 &> /dev/null; then
        local version=$(python3 --version | cut -d' ' -f2)
        if check_version "$version" "3.9.0"; then
            print_success "Python $version is installed (meets minimum requirement of 3.9)"
        else
            print_warning "Python $version is installed but version 3.9+ is required"
            return 1
        fi
    elif command -v python &> /dev/null; then
        local version=$(python --version | cut -d' ' -f2)
        if check_version "$version" "3.9.0"; then
            print_success "Python $version is installed (meets minimum requirement of 3.9)"
        else
            print_warning "Python $version is installed but version 3.9+ is required"
            return 1
        fi
    else
        print_warning "Python is not installed. Please install Python 3.9+ for Python SDK support."
        return 1
    fi
    
    # Check if pip is available
    if command -v pip3 &> /dev/null; then
        PIP_CMD="pip3"
    elif command -v pip &> /dev/null; then
        PIP_CMD="pip"
    else
        print_warning "pip is not installed. Please install pip for Python package management."
        return 1
    fi
    
    # Install conductor-python
    print_status "Installing conductor-python package..."
    
    # Try different installation methods
    if $PIP_CMD install conductor-python; then
        print_success "conductor-python package installed"
    elif $PIP_CMD install --user conductor-python; then
        print_success "conductor-python package installed (user mode)"
    elif python3 -m pip install conductor-python; then
        print_success "conductor-python package installed (python -m pip)"
    elif python3 -m pip install --user conductor-python; then
        print_success "conductor-python package installed (python -m pip --user)"
    else
        print_warning "Failed to install conductor-python via pip. Trying alternative methods..."
        
        # Try creating a virtual environment
        if command -v python3 -m venv &> /dev/null; then
            print_status "Creating virtual environment for Python SDK..."
            python3 -m venv conductor-python-env
            source conductor-python-env/bin/activate
            if pip install conductor-python; then
                print_success "conductor-python package installed in virtual environment"
                print_status "To use Python SDK, activate the environment: source conductor-python-env/bin/activate"
            else
                print_warning "Failed to install in virtual environment too"
                return 1
            fi
        else
            print_warning "Could not install conductor-python. Please install manually:"
            print_status "  pip3 install conductor-python"
            print_status "  or use a virtual environment"
            return 1
        fi
    fi
    
    # Python SDK is integrated directly in SdkTestAutomation.Sdk project
    # Just verify the pythonnet package is available
    print_status "Verifying pythonnet package..."
    
    # Build the main SDK project to verify Python integration
    dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    dotnet build SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    
    print_success "Python SDK setup complete (Python.NET bridge)"
}

# Create environment file
create_env_file() {
    print_status "Creating environment configuration..."
    
    local env_file="SdkTestAutomation.Tests/.env"
    
    # Check if virtual environment was created
    local venv_path=""
    if [ -d "conductor-python-env" ]; then
        venv_path="$(pwd)/conductor-python-env"
    fi
    
    if [ ! -f "$env_file" ]; then
        cat > "$env_file" << EOF
# Conductor Server Configuration
CONDUCTOR_SERVER_URL=http://localhost:8080/api

# SDK Selection (csharp, java, python)
TEST_SDK=csharp

# Python Configuration (if using Python SDK)
PYTHON_HOME=$(which python3 2>/dev/null || which python 2>/dev/null || echo "/usr/local/bin/python3")
PYTHONPATH=$(python3 -c "import site; print(site.getsitepackages()[0])" 2>/dev/null || echo "/usr/local/lib/python3.9/site-packages")
$(if [ -n "$venv_path" ]; then echo "# Python Virtual Environment (if created by setup script)"; echo "PYTHON_VENV_PATH=$venv_path"; fi)

# Java Configuration (if using Java SDK)
JAVA_HOME=\${JAVA_HOME:-$(echo $JAVA_HOME)}
JAVA_CLASSPATH=\${JAVA_CLASSPATH:-""}
EOF
        print_success "Environment file created: $env_file"
    else
        print_warning "Environment file already exists: $env_file"
        if [ -n "$venv_path" ]; then
            print_status "Note: Python virtual environment created at: $venv_path"
            print_status "To use Python SDK, activate it: source $venv_path/bin/activate"
        fi
    fi
}

# Test SDK setup
test_sdk_setup() {
    print_status "Testing SDK setup..."
    
    # Build all projects
    dotnet build
    
    if [ $? -eq 0 ]; then
        print_success "All projects build successfully"
    else
        print_error "Build failed. Please check the errors above."
        return 1
    fi
}

# Main execution
main() {
    echo ""
    print_status "Starting SDK setup..."
    echo ""
    
    # Check prerequisites
    check_dotnet
    
    echo ""
    print_status "Setting up individual SDKs..."
    echo ""
    
    # Setup each SDK
    setup_csharp_sdk
    echo ""
    
    if setup_java_sdk; then
        JAVA_READY=true
    else
        JAVA_READY=false
        print_warning "Java SDK setup incomplete"
    fi
    echo ""
    
    if setup_python_sdk; then
        PYTHON_READY=true
    else
        PYTHON_READY=false
        print_warning "Python SDK setup incomplete"
    fi
    echo ""
    
    # Create environment file
    create_env_file
    echo ""
    
    # Test setup
    test_sdk_setup
    echo ""
    
    # Summary
    echo "📋 Setup Summary:"
    echo "================="
    print_success "✅ C# SDK: Ready to use"
    
    if [ "$JAVA_READY" = true ]; then
        print_success "✅ Java SDK: Ready to use"
    else
        print_warning "⚠️  Java SDK: Needs manual setup"
    fi
    
    if [ "$PYTHON_READY" = true ]; then
        print_success "✅ Python SDK: Ready to use"
    else
        print_warning "⚠️  Python SDK: Needs manual setup"
    fi
    
    echo ""
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Update SdkTestAutomation.Tests/.env with your server URL"
    echo "3. Run tests: TEST_SDK=csharp dotnet test SdkTestAutomation.Tests"
    echo ""
    print_success "Setup complete! 🎉"
}

# Run main function
main "$@" 