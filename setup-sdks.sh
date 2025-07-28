#!/bin/bash

# Comprehensive SDK Setup Script for SdkTestAutomation
# This script handles setup, testing, and building for all four Conductor SDKs

set -e

echo "🚀 SdkTestAutomation Comprehensive SDK Setup Script"
echo "=================================================="

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

# Version check function
check_version() {
    local version=$1
    local required=$2
    if [ "$(printf '%s\n' "$required" "$version" | sort -V | head -n1)" = "$required" ]; then 
        return 0
    else
        return 1
    fi
}

# Platform detection function
detect_platform() {
    case "$(uname -s)" in
        Linux*)
            echo "linux"
            ;;
        Darwin*)
            echo "darwin"
            ;;
        CYGWIN*|MINGW*|MSYS*)
            echo "windows"
            ;;
        *)
            echo "unknown"
            ;;
    esac
}

# Get library name for platform
get_library_name() {
    local platform=$1
    case "$platform" in
        linux)
            echo "conductor-go-bridge.so"
            ;;
        darwin)
            echo "conductor-go-bridge.dylib"
            ;;
        windows)
            echo "conductor-go-bridge.dll"
            ;;
        *)
            echo "conductor-go-bridge.dll"
            ;;
    esac
}

# Check if Go is installed and version
check_go_installation() {
    if command -v go &> /dev/null; then
        local version=$(go version | cut -d' ' -f3 | sed 's/go//')
        if check_version "$version" "1.19.0"; then
            print_success "Go $version is installed (meets minimum requirement of 1.19)"
            return 0
        else
            print_warning "Go $version is installed but version 1.19+ is required"
            return 1
        fi
    else
        print_warning "Go is not installed. Please install Go 1.19+ for Go SDK support."
        return 1
    fi
}

# Check if Go is installed (simple version for test scripts)
check_go_installation_simple() {
    if command -v go &> /dev/null; then
        local version=$(go version | cut -d' ' -f3 | sed 's/go//')
        print_success "Go $version is installed"
        return 0
    else
        print_error "Go is not installed"
        return 1
    fi
}

# Check if shared library exists
check_shared_library() {
    local go_dir="SdkTestAutomation.Sdk/Implementations/Go"
    local platform=$(detect_platform)
    local library_name=$(get_library_name "$platform")
    
    if [ -f "$go_dir/$library_name" ]; then
        print_success "Shared library found: $library_name"
        return 0
    else
        print_warning "Shared library not found: $library_name"
        return 1
    fi
}

# Safe directory change with return
safe_cd() {
    local target_dir=$1
    if [ -d "$target_dir" ]; then
        cd "$target_dir"
        return 0
    else
        print_error "Directory not found: $target_dir"
        return 1
    fi
}

# Return to original directory
return_to_root() {
    cd - > /dev/null
}

# Check if .NET is installed
check_dotnet() {
    print_status "Checking .NET installation..."
    if command -v dotnet &> /dev/null; then
        local version=$(dotnet --version)
        print_success ".NET $version is installed"
        return 0
    else
        print_error ".NET is not installed. Please install .NET 8.0 or later."
        return 1
    fi
}

# Check if Java is installed with version
check_java_installation() {
    if command -v java &> /dev/null; then
        local version=$(java -version 2>&1 | head -n 1 | cut -d'"' -f2)
        if check_version "$version" "17.0.0"; then
            print_success "Java $version is installed (meets minimum requirement of 17)"
            return 0
        else
            print_warning "Java $version is installed but version 17+ is required"
            return 1
        fi
    else
        print_warning "Java is not installed. Please install Java 17+ for Java SDK support."
        return 1
    fi
}

# Check if Python is installed with version
check_python_installation() {
    if command -v python3 &> /dev/null; then
        local version=$(python3 --version | cut -d' ' -f2)
        if check_version "$version" "3.9.0"; then
            print_success "Python $version is installed (meets minimum requirement of 3.9)"
            return 0
        else
            print_warning "Python $version is installed but version 3.9+ is required"
            return 1
        fi
    elif command -v python &> /dev/null; then
        local version=$(python --version | cut -d' ' -f2)
        if check_version "$version" "3.9.0"; then
            print_success "Python $version is installed (meets minimum requirement of 3.9)"
            return 0
        else
            print_warning "Python $version is installed but version 3.9+ is required"
            return 1
        fi
    else
        print_warning "Python is not installed. Please install Python 3.9+ for Python SDK support."
        return 1
    fi
}

# Get pip command
get_pip_command() {
    if command -v pip3 &> /dev/null; then
        echo "pip3"
    elif command -v pip &> /dev/null; then
        echo "pip"
    else
        echo ""
    fi
}

# Clean up Go artifacts
cleanup_go_artifacts() {
    local go_dir="SdkTestAutomation.Sdk/Implementations/Go"
    
    # Remove old shared libraries
    rm -f "$go_dir/conductor-go-bridge.dll"
    rm -f "$go_dir/conductor-go-bridge.so"
    rm -f "$go_dir/conductor-go-bridge.dylib"
    
    # Remove any temporary build artifacts
    rm -f "$go_dir/conductor-go-bridge.h"
    
    print_status "Cleaned up Go artifacts"
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --setup-only      Only setup SDKs (default)"
    echo "  --test-only       Only test existing SDKs"
    echo "  --build-only      Only build Go shared library"
    echo "  --full            Setup, test, and build everything"
    echo "  --help            Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0                # Setup all SDKs"
    echo "  $0 --test-only    # Test existing SDKs"
    echo "  $0 --build-only   # Build Go shared library"
    echo "  $0 --full         # Complete setup, test, and build"
}

# Parse command line arguments
MODE="setup"

while [[ $# -gt 0 ]]; do
    case $1 in
        --setup-only)
            MODE="setup"
            shift
            ;;
        --test-only)
            MODE="test"
            shift
            ;;
        --build-only)
            MODE="build"
            shift
            ;;
        --full)
            MODE="full"
            shift
            ;;
        --help)
            show_usage
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Common .NET build function
build_dotnet_project() {
    local project_path="SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj"
    if dotnet build "$project_path" --no-restore &> /dev/null; then
        return 0
    else
        return 1
    fi
}

# Setup C# SDK
setup_csharp_sdk() {
    print_status "Setting up C# SDK..."
    
    # C# SDK is integrated directly in SdkTestAutomation.Sdk project
    print_status "Verifying conductor-csharp package..."
    
    # Build the main SDK project to verify C# integration
    dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    if build_dotnet_project; then
        print_success "C# SDK setup complete (conductor-csharp v1.1.3)"
        return 0
    else
        print_error "C# SDK setup failed"
        return 1
    fi
}

# Setup Java SDK
setup_java_sdk() {
    print_status "Setting up Java SDK..."
    
    # Check if Java is installed with version check
    if ! check_java_installation; then
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
        if curl -L -o "$jar_dir/conductor-client.jar" "https://repo1.maven.org/maven2/com/netflix/conductor/conductor-client/4.0.12/conductor-client-4.0.12.jar" && \
           curl -L -o "$jar_dir/conductor-common.jar" "https://repo1.maven.org/maven2/com/netflix/conductor/conductor-common/4.0.12/conductor-common-4.0.12.jar"; then
            print_success "JAR files downloaded successfully"
        else
            print_warning "Failed to download JAR files automatically"
            print_status "Please download manually from https://github.com/conductor-oss/java-sdk/releases"
            return 1
        fi
    fi
    
    # Build Java project
    dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    if build_dotnet_project; then
        print_success "Java SDK setup complete (IKVM.NET bridge)"
        return 0
    else
        print_error "Java SDK setup failed"
        return 1
    fi
}

# Setup Python SDK
setup_python_sdk() {
    print_status "Setting up Python SDK..."
    
    # Check if Python is installed with version check
    if ! check_python_installation; then
        return 1
    fi
    
    # Check if pip is available
    PIP_CMD=$(get_pip_command)
    if [ -z "$PIP_CMD" ]; then
        print_warning "pip is not installed. Please install pip for Python package management."
        return 1
    fi
    
    # Install conductor-python
    print_status "Installing conductor-python package..."
    
    # Try different installation methods
    local installed=false
    for cmd in "$PIP_CMD install conductor-python" \
               "$PIP_CMD install --user conductor-python" \
               "python3 -m pip install conductor-python" \
               "python3 -m pip install --user conductor-python"; do
        if eval "$cmd" &> /dev/null; then
            print_success "conductor-python package installed"
            installed=true
            break
        fi
    done
    
    if [ "$installed" = false ]; then
        print_warning "Failed to install conductor-python via pip. Trying virtual environment..."
        
        # Try creating a virtual environment
        if command -v python3 -m venv &> /dev/null; then
            print_status "Creating virtual environment for Python SDK..."
            python3 -m venv conductor-python-env
            source conductor-python-env/bin/activate
            if pip install conductor-python &> /dev/null; then
                print_success "conductor-python package installed in virtual environment"
                print_status "To use Python SDK, activate the environment: source conductor-python-env/bin/activate"
                installed=true
            else
                print_warning "Failed to install in virtual environment too"
            fi
        else
            print_warning "Could not install conductor-python. Please install manually:"
            print_status "  pip3 install conductor-python"
            print_status "  or use a virtual environment"
            return 1
        fi
    fi
    
    # Python SDK is integrated directly in SdkTestAutomation.Sdk project
    print_status "Verifying pythonnet package..."
    
    # Build the main SDK project to verify Python integration
    dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    if build_dotnet_project; then
        print_success "Python SDK setup complete (Python.NET bridge)"
        return 0
    else
        print_error "Python SDK setup failed"
        return 1
    fi
}

# Setup Go SDK
setup_go_sdk() {
    print_status "Setting up Go SDK..."
    
    # Clean up any existing artifacts
    cleanup_go_artifacts
    
    # Check Go installation
    if ! check_go_installation; then
        return 1
    fi
    
    # Setup Go module in the implementation directory
    local go_dir="SdkTestAutomation.Sdk/Implementations/Go"
    print_status "Setting up Go module in $go_dir..."
    
    # Ensure the directory exists
    if [ ! -d "$go_dir" ]; then
        print_error "Go implementation directory not found: $go_dir"
        return 1
    fi
    
    # Change to Go directory
    if ! safe_cd "$go_dir"; then
        return 1
    fi
    
    # Check if go.mod exists, if not create it
    if [ ! -f "go.mod" ]; then
        print_status "Creating go.mod file..."
        if ! go mod init conductor-go-bridge; then
            print_error "Failed to initialize Go module"
            return_to_root
            return 1
        fi
    fi
    
    # Update go.mod with correct dependencies
    print_status "Updating Go dependencies..."
    
    # Add conductor-go SDK dependency
    if ! go list -m github.com/conductor-sdk/conductor-go &> /dev/null; then
        print_status "Adding conductor-go SDK dependency..."
        if ! go get github.com/conductor-sdk/conductor-go@latest; then
            print_error "Failed to add conductor-go SDK dependency"
            return_to_root
            return 1
        fi
    else
        print_status "conductor-go SDK dependency already exists"
    fi
    
    # Add gorilla/mux dependency for HTTP server (if needed)
    if ! go list -m github.com/gorilla/mux &> /dev/null; then
        print_status "Adding gorilla/mux dependency..."
        if ! go get github.com/gorilla/mux@latest; then
            print_warning "Failed to add gorilla/mux dependency (optional)"
        fi
    else
        print_status "gorilla/mux dependency already exists"
    fi
    
    # Tidy up dependencies
    print_status "Tidying Go dependencies..."
    if ! go mod tidy; then
        print_error "Failed to tidy Go dependencies"
        return_to_root
        return 1
    fi
    
    # Verify dependencies are properly installed
    print_status "Downloading Go dependencies..."
    if ! go mod download; then
        print_error "Failed to download Go dependencies"
        return_to_root
        return 1
    fi
    
    print_success "Go dependencies installed successfully"
    
    # Return to original directory
    return_to_root
    
    # Build Go shared library for better performance
    print_status "Building Go shared library..."
    
    # Set environment variables for CGO
    export CGO_ENABLED=1
    
    # Detect platform and set appropriate build flags
    local platform=$(detect_platform)
    local library_name=$(get_library_name "$platform")
    
    export GOOS="$platform"
    export GOARCH=amd64
    
    print_status "Building for platform: $platform ($library_name)"
    
    # Build the shared library from the Go directory
    local shared_library_built=false
    if go build -buildmode=c-shared -o "$library_name" "$go_dir/conductor-go-bridge.go"; then
        print_success "✅ Successfully built $library_name"
        
        # Copy to the appropriate location
        cp "$library_name" "$go_dir/"
        print_success "📋 Copied to $go_dir/"
        
        # Show file info
        ls -la "$library_name"
        
        # Clean up the root directory copy
        rm -f "$library_name"
        
        shared_library_built=true
        print_success "Go shared library build complete!"
    else
        print_warning "Failed to build Go shared library"
        print_status "Trying alternative build approach..."
        
        # Try building from the Go directory directly
        if safe_cd "$go_dir"; then
            if go build -buildmode=c-shared -o "$library_name" conductor-go-bridge.go; then
                print_success "✅ Successfully built $library_name (alternative method)"
                shared_library_built=true
            else
                print_error "Failed to build Go shared library with both methods"
                return_to_root
                return 1
            fi
            return_to_root
        else
            return 1
        fi
    fi
    
    # Verify the Go integration works
    print_status "Verifying Go SDK integration..."
    
    # Test the shared library
    if [ "$shared_library_built" = true ]; then
        print_status "Testing shared library functionality..."
        
        # Check if the library can be loaded (basic test)
        if check_shared_library; then
            local library_path="$go_dir/$library_name"
            print_status "Library size: $(ls -lh "$library_path" | awk '{print $5}')"
        fi
    fi
    
    # Build the main SDK project to verify Go integration
    print_status "Building .NET SDK project to verify integration..."
    dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj
    if build_dotnet_project; then
        if [ "$shared_library_built" = true ]; then
            print_success "Go SDK setup complete (Shared Library)"

            return 0
        else
            print_error "Go SDK setup failed - shared library is required"
            return 1
        fi
    else
        print_error "Go SDK .NET integration failed"
        return 1
    fi
}

# Test C# SDK
test_csharp_sdk() {
    print_status "Testing C# SDK..."
    
    if build_dotnet_project; then
        print_success "C# SDK builds successfully"
        return 0
    else
        print_error "C# SDK build failed"
        return 1
    fi
}

# Test Java SDK
test_java_sdk() {
    print_status "Testing Java SDK..."
    
    # Check if Java is installed
    if ! check_java_installation; then
        print_warning "Java SDK test skipped - Java not installed"
        return 1
    fi
    
    # Check JAR files
    local jar_dir="SdkTestAutomation.Sdk/lib"
    if [ -f "$jar_dir/conductor-client.jar" ] && [ -f "$jar_dir/conductor-common.jar" ]; then
        print_success "Java JAR files found"
    else
        print_warning "Java JAR files not found"
        return 1
    fi
    
    # Test .NET build
    if build_dotnet_project; then
        print_success "Java SDK builds successfully"
        return 0
    else
        print_error "Java SDK build failed"
        return 1
    fi
}

# Test Python SDK
test_python_sdk() {
    print_status "Testing Python SDK..."
    
    # Check if Python is installed
    if ! check_python_installation; then
        print_warning "Python SDK test skipped - Python not installed"
        return 1
    fi
    
    # Check if conductor-python is installed (try multiple locations)
    local conductor_found=false
    
    # Try global Python environment
    if python3 -c "import conductor" &> /dev/null 2>&1 || python -c "import conductor" &> /dev/null 2>&1; then
        print_success "conductor-python package found (global environment)"
        conductor_found=true
    fi
    
    # Try virtual environment if it exists
    if [ "$conductor_found" = false ] && [ -d "conductor-python-env" ]; then
        if source conductor-python-env/bin/activate && python -c "import conductor" &> /dev/null 2>&1; then
            print_success "conductor-python package found (virtual environment)"
            conductor_found=true
        fi
    fi
    
    if [ "$conductor_found" = false ]; then
        print_warning "conductor-python package not found in any environment"
        return 1
    fi
    
    # Test .NET build
    if build_dotnet_project; then
        print_success "Python SDK builds successfully"
        return 0
    else
        print_error "Python SDK build failed"
        return 1
    fi
}

# Test Go SDK
test_go_sdk() {
    print_status "Testing Go SDK..."
    
    # Check if Go is installed
    if ! check_go_installation_simple; then
        print_warning "Go SDK test skipped - Go not installed"
        return 1
    fi
    
    # Test Go module setup
    local go_dir="SdkTestAutomation.Sdk/Implementations/Go"
    if [ ! -d "$go_dir" ]; then
        print_error "Go implementation directory not found: $go_dir"
        return 1
    fi
    
    if [ ! -f "$go_dir/go.mod" ]; then
        print_error "go.mod file not found"
        return 1
    fi
    
    print_success "Go module setup found"
    
    # Test dependencies
    if ! safe_cd "$go_dir"; then
        return 1
    fi
    
    if go list -m github.com/conductor-sdk/conductor-go &> /dev/null; then
        print_success "conductor-go SDK dependency found"
    else
        print_error "conductor-go SDK dependency missing"
        return_to_root
        return 1
    fi
    
    if go list -m github.com/gorilla/mux &> /dev/null; then
        print_success "gorilla/mux dependency found"
    else
        print_warning "gorilla/mux dependency missing (optional)"
    fi
    
    # Test compilation
    if go build -o /dev/null conductor-go-bridge.go 2>/dev/null; then
        print_success "Go code compiles successfully"
    else
        print_error "Go code compilation failed"
        return_to_root
        return 1
    fi
    
    return_to_root
    
    # Test shared library
    if check_shared_library; then
        print_success "Go shared library found"
    else
        print_warning "Go shared library not found"
    fi
    
    # Test .NET integration
    if build_dotnet_project; then
        print_success "Go SDK .NET integration builds successfully"
        return 0
    else
        print_error "Go SDK .NET integration build failed"
        return 1
    fi
}

# Build Go shared library only
build_go_library() {
    print_status "Building Go shared library..."
    
    # Check if Go is installed
    if ! check_go_installation_simple; then
        print_error "Please install Go 1.19+ first."
        return 1
    fi
    
    # Set environment variables for CGO
    export CGO_ENABLED=1
    
    # Detect platform and set appropriate build flags
    local platform=$(detect_platform)
    local library_name=$(get_library_name "$platform")
    
    export GOOS="$platform"
    export GOARCH=amd64
    
    print_status "Building for platform: $platform ($library_name)"
    
    # Change to the Go implementation directory
    if ! safe_cd "SdkTestAutomation.Sdk/Implementations/Go/"; then
        return 1
    fi
    
    # Ensure go.mod exists and dependencies are up to date
    if [ ! -f "go.mod" ]; then
        print_error "go.mod file not found. Please run setup first."
        return_to_root
        return 1
    fi
    
    # Update dependencies
    print_status "Updating Go dependencies..."
    go mod tidy
    go mod download
    
    # Verify conductor-go SDK is available
    if ! go list -m github.com/conductor-sdk/conductor-go &> /dev/null; then
        print_error "conductor-go SDK not found. Please run setup first."
        return_to_root
        return 1
    fi
    
    # Build the shared library
    print_status "Compiling $library_name..."
    if go build -buildmode=c-shared -o "$library_name" conductor-go-bridge.go; then
        print_success "✅ Successfully built $library_name"
        print_status "📁 Library location: $(pwd)/$library_name"
        
        # Show file info
        ls -la "$library_name"
        
        # Return to original directory
        return_to_root
        
        print_status "📋 Library is ready for use in SdkTestAutomation.Sdk/Implementations/Go/"
        return 0
    else
        print_error "❌ Failed to build Go shared library"
        return_to_root
        return 1
    fi
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

# SDK Selection (csharp, java, python, go)
SDK_TYPE=csharp

# Python Configuration (if using Python SDK)
PYTHON_HOME=$(which python3 2>/dev/null || which python 2>/dev/null || echo "/usr/local/bin/python3")
PYTHONPATH=$(python3 -c "import site; print(site.getsitepackages()[0])" 2>/dev/null || echo "/usr/local/lib/python3.9/site-packages")
$(if [ -n "$venv_path" ]; then echo "# Python Virtual Environment (if created by setup script)"; echo "PYTHON_VENV_PATH=$venv_path"; fi)

# Java Configuration (if using Java SDK)
JAVA_HOME=\${JAVA_HOME:-$(echo $JAVA_HOME)}
JAVA_CLASSPATH=\${JAVA_CLASSPATH:-""}

# Go Configuration (if using Go SDK)
GOPATH=\${GOPATH:-$(go env GOPATH 2>/dev/null || echo "$HOME/go")}
GOROOT=\${GOROOT:-$(go env GOROOT 2>/dev/null || echo "/usr/local/go")}
# Go Shared Library: conductor-go-bridge.dll/.so/.dylib (auto-built during setup)
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
    if dotnet build; then
        print_success "All projects build successfully"
        return 0
    else
        print_error "Build failed. Please check the errors above."
        return 1
    fi
}

# Print summary
print_summary() {
    local title="$1"
    local java_ready="$2"
    local python_ready="$3"
    local go_ready="$4"
    local go_shared_library="$5"
    
    echo ""
    echo "📋 $title"
    echo "================="
    print_success "✅ C# SDK: Ready to use"
    
    if [ "$java_ready" = true ]; then
        print_success "✅ Java SDK: Ready to use"
    else
        print_warning "⚠️  Java SDK: Needs manual setup"
    fi
    
    if [ "$python_ready" = true ]; then
        print_success "✅ Python SDK: Ready to use"
    else
        print_warning "⚠️  Python SDK: Needs manual setup"
    fi
    
    if [ "$go_ready" = true ]; then
        if [ "$go_shared_library" = true ]; then
            print_success "✅ Go SDK: Ready to use (Shared Library)"
        else
            print_warning "⚠️  Go SDK: Shared library build failed"
        fi
    else
        print_warning "⚠️  Go SDK: Needs manual setup"
    fi
}

# Main execution
main() {
    echo ""
    
    case "$MODE" in
        "test")
            print_status "Running SDK tests only..."
            echo ""
            
            # Test each SDK
            test_csharp_sdk
            echo ""
            
            test_java_sdk
            echo ""
            
            test_python_sdk
            echo ""
            
            test_go_sdk
            echo ""
            
            print_success "✅ SDK tests completed!"
            ;;
            
        "build")
            print_status "Building Go shared library only..."
            echo ""
            
            if build_go_library; then
                print_success "✅ Go shared library build completed!"
            else
                print_error "❌ Go shared library build failed"
                exit 1
            fi
            ;;
            
        "full")
            print_status "Running full setup, test, and build..."
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
            
            if setup_go_sdk; then
                GO_READY=true
                # Check if shared library was built
                if check_shared_library; then
                    GO_SHARED_LIBRARY=true
                else
                    GO_SHARED_LIBRARY=false
                fi
            else
                GO_READY=false
                GO_SHARED_LIBRARY=false
                print_warning "Go SDK setup incomplete"
            fi
            echo ""
            
            # Create environment file
            create_env_file
            echo ""
            
            # Test setup
            test_sdk_setup
            echo ""
            
            # Test each SDK
            print_status "Testing all SDKs..."
            echo ""
            
            test_csharp_sdk
            echo ""
            
            test_java_sdk
            echo ""
            
            test_python_sdk
            echo ""
            
            test_go_sdk
            echo ""
            
            # Summary
            print_summary "Full Setup Summary" "$JAVA_READY" "$PYTHON_READY" "$GO_READY" "$GO_SHARED_LIBRARY"
            ;;
            
        *)
            # Default: Setup only
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
            
            if setup_go_sdk; then
                GO_READY=true
                # Check if shared library was built
                if check_shared_library; then
                    GO_SHARED_LIBRARY=true
                else
                    GO_SHARED_LIBRARY=false
                fi
            else
                GO_READY=false
                GO_SHARED_LIBRARY=false
                print_warning "Go SDK setup incomplete"
            fi
            echo ""
            
            # Create environment file
            create_env_file
            echo ""
            
            # Test setup
            test_sdk_setup
            echo ""
            
            # Summary
            print_summary "Setup Summary" "$JAVA_READY" "$PYTHON_READY" "$GO_READY" "$GO_SHARED_LIBRARY"
            ;;
    esac
    
    echo ""
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Update SdkTestAutomation.Tests/.env with your server URL"
    echo "3. Run tests: SDK_TYPE=csharp dotnet test SdkTestAutomation.Tests"
    if [ "$GO_SHARED_LIBRARY" = true ]; then
        echo "4. Go SDK: Run tests with: SDK_TYPE=go dotnet test SdkTestAutomation.Tests"
    fi
    echo ""
    print_success "Operation complete! 🎉"
}

# Run main function
main "$@" 