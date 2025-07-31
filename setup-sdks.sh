#!/bin/bash

# SdkTestAutomation SDK Setup Script
set -e

echo "🚀 SdkTestAutomation SDK Setup Script"
echo "====================================="

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_status() { echo -e "${BLUE}[INFO]${NC} $1"; }
print_success() { echo -e "${GREEN}[SUCCESS]${NC} $1"; }
print_warning() { echo -e "${YELLOW}[WARNING]${NC} $1"; }
print_error() { echo -e "${RED}[ERROR]${NC} $1"; }

# Version check
check_version() {
    local version=$1
    local required=$2
    [ "$(printf '%s\n' "$required" "$version" | sort -V | head -n1)" = "$required" ]
}

# Platform detection
detect_platform() {
    case "$(uname -s)" in
        Linux*) echo "linux" ;;
        Darwin*) echo "darwin" ;;
        CYGWIN*|MINGW*|MSYS*) echo "windows" ;;
        *) echo "unknown" ;;
    esac
}

get_library_name() {
    case "$1" in
        linux) echo "conductor-go-bridge.so" ;;
        darwin) echo "conductor-go-bridge.dylib" ;;
        windows) echo "conductor-go-bridge.dll" ;;
        *) echo "conductor-go-bridge.dll" ;;
    esac
}

# Prerequisites check
check_go() {
    if command -v go &> /dev/null; then
        local version=$(go version | cut -d' ' -f3 | sed 's/go//')
        if check_version "$version" "1.19.0"; then
            print_success "Go $version is installed"
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

check_java() {
    if command -v java &> /dev/null; then
        local version=$(java -version 2>&1 | head -n 1 | cut -d'"' -f2)
        if check_version "$version" "21.0.0"; then
            print_success "Java $version is installed"
            return 0
        else
            print_warning "Java $version is installed but version 21+ is required"
            return 1
        fi
    else
        print_warning "Java is not installed. Please install Java 21+ for Java SDK support."
        return 1
    fi
}

check_python() {
    if command -v python3 &> /dev/null; then
        local version=$(python3 --version | cut -d' ' -f2)
        if check_version "$version" "3.9.0"; then
            print_success "Python $version is installed"
            return 0
        else
            print_warning "Python $version is installed but version 3.9+ is required"
            return 1
        fi
    elif command -v python &> /dev/null; then
        local version=$(python --version | cut -d' ' -f2)
        if check_version "$version" "3.9.0"; then
            print_success "Python $version is installed"
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

check_dotnet() {
    if command -v dotnet &> /dev/null; then
        local version=$(dotnet --version)
        print_success ".NET $version is installed"
        return 0
    else
        print_error ".NET is not installed. Please install .NET 8.0 or later."
        return 1
    fi
}

# Go shared library check
check_shared_library() {
    local go_dir="SdkTestAutomation.Sdk/Implementations/Go"
    local build_artifacts_dir="$go_dir/build-artifacts"
    local platform=$(detect_platform)
    local library_name=$(get_library_name "$platform")
    
    if [ -f "$build_artifacts_dir/$library_name" ]; then
        print_success "Shared library found: $build_artifacts_dir/$library_name"
        return 0
    else
        print_warning "Shared library not found: $build_artifacts_dir/$library_name"
        return 1
    fi
}

# Clean up Go artifacts
cleanup_go_artifacts() {
    local go_dir="SdkTestAutomation.Sdk/Implementations/Go"
    local build_artifacts_dir="$go_dir/build-artifacts"
    local go_src_dir="$go_dir/go-src"
    
    # Remove build artifacts directory
    if [ -d "$build_artifacts_dir" ]; then
        rm -rf "$build_artifacts_dir"
        print_status "Removed build artifacts directory"
    fi
    
    # Remove any artifacts from go-src directory
    rm -f "$go_src_dir/conductor-go-bridge.dll"
    rm -f "$go_src_dir/conductor-go-bridge.so"
    rm -f "$go_src_dir/conductor-go-bridge.dylib"
    rm -f "$go_src_dir/conductor-go-bridge"
    rm -f "$go_src_dir/conductor-go-bridge.h"
    
    # Remove any artifacts from root Go directory (legacy cleanup)
    rm -f "$go_dir/conductor-go-bridge.dll"
    rm -f "$go_dir/conductor-go-bridge.so"
    rm -f "$go_dir/conductor-go-bridge.dylib"
    rm -f "$go_dir/conductor-go-bridge"
    
    print_status "Cleaned up Go artifacts"
}

# Build Go shared library
build_go_library() {
    local go_dir="SdkTestAutomation.Sdk/Implementations/Go"
    local go_src_dir="$go_dir/go-src"
    local original_dir=$(pwd)
    
    if [ ! -d "$go_src_dir" ]; then
        print_error "Go source directory not found: $go_src_dir"
        return 1
    fi
    
    cd "$go_src_dir"
    
    # Ensure go.mod exists
    if [ ! -f "go.mod" ]; then
        print_status "Creating go.mod file..."
        if ! go mod init conductor-go-bridge; then
            print_error "Failed to initialize Go module"
            cd "$original_dir"
            return 1
        fi
    fi
    
    # Add conductor-go SDK dependency
    if ! go list -m github.com/conductor-sdk/conductor-go &> /dev/null; then
        print_status "Adding conductor-go SDK dependency..."
        if ! go get github.com/conductor-sdk/conductor-go@latest; then
            print_error "Failed to add conductor-go SDK dependency"
            cd "$original_dir"
            return 1
        fi
    fi
    
    # Tidy and download dependencies
    print_status "Updating Go dependencies..."
    go mod tidy
    go mod download
    
    # Set build environment
    export CGO_ENABLED=1
    local platform=$(detect_platform)
    local library_name=$(get_library_name "$platform")
    export GOOS="$platform"
    
    # Set architecture
    if [ "$platform" = "darwin" ]; then
        if [ "$(uname -m)" = "arm64" ]; then
            export GOARCH=arm64
            print_status "Building for Apple Silicon (ARM64)"
        else
            export GOARCH=amd64
            print_status "Building for Intel Mac (AMD64)"
        fi
    elif [ "$platform" = "linux" ]; then
        if [ "$(uname -m)" = "aarch64" ]; then
            export GOARCH=arm64
            print_status "Building for ARM64 Linux"
        else
            export GOARCH=amd64
            print_status "Building for AMD64 Linux"
        fi
    else
        export GOARCH=amd64
        print_status "Building for AMD64 (default)"
    fi
    
    # Create build-artifacts directory
    mkdir -p "../build-artifacts"
    
    # Build shared library directly into build-artifacts
    print_status "Building $library_name..."
    if go build -buildmode=c-shared -o "../build-artifacts/$library_name" conductor-go-bridge.go; then
        print_success "Successfully built $library_name"
        cd "$original_dir"
        return 0
    else
        print_error "Failed to build Go shared library"
        cd "$original_dir"
        return 1
    fi
}

# Setup functions
setup_csharp_sdk() {
    print_status "Setting up C# SDK..."
    if dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj; then
        print_success "C# SDK setup complete"
        return 0
    else
        print_error "C# SDK setup failed"
        return 1
    fi
}

setup_java_sdk() {
    print_status "Setting up Java SDK..."
    
    if ! check_java; then
        return 1
    fi
    
    # Check if Maven is installed
    if ! command -v mvn &> /dev/null; then
        print_error "Maven is not installed. Please install Maven for Java SDK support."
        print_status "Install with: brew install maven (macOS) or download from maven.apache.org"
        return 1
    fi
    
    local java_cli_dir="SdkTestAutomation.Sdk/Implementations/Java/cli-java-sdk"
    local jar_dir="SdkTestAutomation.Sdk/bin/Debug/net8.0/lib"
    local original_dir=$(pwd)
    
    # Always build Java CLI applications to ensure they're up to date
    print_status "Building Java CLI applications..."
    
    if [ ! -d "$java_cli_dir" ]; then
        print_error "Java CLI directory not found: $java_cli_dir"
        return 1
    fi
    
    cd "$java_cli_dir"
    
    # Make build script executable
    chmod +x build.sh
    
    # Clean previous builds
    print_status "Cleaning previous builds..."
    if ! mvn clean &> /dev/null; then
        print_warning "Failed to clean previous builds, continuing..."
    fi
    
    # Build Java CLI applications
    print_status "Building Java CLI applications with Maven..."
    if ./build.sh; then
        print_success "Java CLI applications built successfully"
        
        # Return to original directory for verification
        cd "$original_dir"
        
        # Wait a moment for file system to sync
        sleep 1
        
        # Verify JAR files were created (after build.sh has copied them)
        if [ -f "$jar_dir/conductor-client.jar" ] && [ -f "$jar_dir/orkes-conductor-client.jar" ]; then
            print_success "Java CLI JAR files verified:"
            print_status "  • conductor-client.jar ($(ls -lh "$jar_dir/conductor-client.jar" | awk '{print $5}'))"
            print_status "  • orkes-conductor-client.jar ($(ls -lh "$jar_dir/orkes-conductor-client.jar" | awk '{print $5}'))"
        else
            print_error "JAR files not found in expected location: $jar_dir"
            print_status "Expected files:"
            print_status "  • $jar_dir/conductor-client.jar"
            print_status "  • $jar_dir/orkes-conductor-client.jar"
            print_status "Available files in $jar_dir:"
            ls -la "$jar_dir" 2>/dev/null || print_status "  (directory not found)"
            return 1
        fi
    else
        print_error "Failed to build Java CLI applications"
        cd "$original_dir"
        return 1
    fi
    
    # Test the built JAR files
    print_status "Testing Java CLI applications..."
    if java -jar "$jar_dir/conductor-client.jar" --help &> /dev/null; then
        print_success "Conductor CLI application test passed"
    else
        print_warning "Conductor CLI application test failed"
    fi
    
    if java -jar "$jar_dir/orkes-conductor-client.jar" --help &> /dev/null; then
        print_success "Orkes CLI application test passed"
    else
        print_warning "Orkes CLI application test failed"
    fi
    
    if dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj; then
        print_success "Java SDK setup complete"
        return 0
    else
        print_error "Java SDK setup failed"
        return 1
    fi
}

setup_python_sdk() {
    print_status "Setting up Python SDK..."
    
    if ! check_python; then
        return 1
    fi
    
    # Try to install conductor-python
    local installed=false
    for cmd in "pip3 install conductor-python" "pip install conductor-python" "python3 -m pip install conductor-python"; do
        if eval "$cmd" &> /dev/null; then
            print_success "conductor-python package installed"
            installed=true
            break
        fi
    done
    
    if [ "$installed" = false ]; then
        print_warning "Failed to install conductor-python via pip"
        if command -v python3 -m venv &> /dev/null; then
            print_status "Creating virtual environment..."
            python3 -m venv conductor-python-env
            source conductor-python-env/bin/activate
            if pip install conductor-python &> /dev/null; then
                print_success "conductor-python installed in virtual environment"
                print_status "Activate with: source conductor-python-env/bin/activate"
                installed=true
            fi
        fi
    fi
    
    if [ "$installed" = false ]; then
        print_warning "Could not install conductor-python"
        return 1
    fi
    
    if dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj; then
        print_success "Python SDK setup complete"
        return 0
    else
        print_error "Python SDK setup failed"
        return 1
    fi
}

setup_go_sdk() {
    print_status "Setting up Go SDK..."
    
    cleanup_go_artifacts
    
    if ! check_go; then
        return 1
    fi
    
    if build_go_library; then
        if dotnet restore SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj && \
           dotnet build SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj --no-restore &> /dev/null; then
            print_success "Go SDK setup complete"
            return 0
        else
            print_error "Go SDK .NET integration failed"
            return 1
        fi
    else
        print_error "Go SDK setup failed"
        return 1
    fi
}

# Test functions
test_csharp_sdk() {
    print_status "Testing C# SDK..."
    if dotnet build SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj --no-restore &> /dev/null; then
        print_success "C# SDK builds successfully"
        return 0
    else
        print_error "C# SDK build failed"
        return 1
    fi
}

test_java_sdk() {
    print_status "Testing Java SDK..."
    if ! check_java; then
        print_warning "Java SDK test skipped - Java not installed"
        return 1
    fi
    
    local jar_dir="SdkTestAutomation.Sdk/bin/Debug/net8.0/lib"
    if [ -f "$jar_dir/conductor-client.jar" ] && [ -f "$jar_dir/orkes-conductor-client.jar" ]; then
        print_success "Java CLI JAR files found"
        
        # Test JAR file sizes
        local conductor_size=$(ls -lh "$jar_dir/conductor-client.jar" | awk '{print $5}')
        local orkes_size=$(ls -lh "$jar_dir/orkes-conductor-client.jar" | awk '{print $5}')
        print_status "  • conductor-client.jar: $conductor_size"
        print_status "  • orkes-conductor-client.jar: $orkes_size"
        
        # Test JAR file execution
        print_status "Testing JAR file execution..."
        if java -jar "$jar_dir/conductor-client.jar" --help &> /dev/null; then
            print_success "Conductor CLI application test passed"
        else
            print_warning "Conductor CLI application test failed"
        fi
        
        if java -jar "$jar_dir/orkes-conductor-client.jar" --help &> /dev/null; then
            print_success "Orkes CLI application test passed"
        else
            print_warning "Orkes CLI application test failed"
        fi
    else
        print_warning "Java CLI JAR files not found"
        print_status "Expected files:"
        print_status "  • $jar_dir/conductor-client.jar"
        print_status "  • $jar_dir/orkes-conductor-client.jar"
        return 1
    fi
    
    if dotnet build SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj --no-restore &> /dev/null; then
        print_success "Java SDK builds successfully"
        return 0
    else
        print_error "Java SDK build failed"
        return 1
    fi
}

test_python_sdk() {
    print_status "Testing Python SDK..."
    if ! check_python; then
        print_warning "Python SDK test skipped - Python not installed"
        return 1
    fi
    
    local conductor_found=false
    if python3 -c "import conductor" &> /dev/null 2>&1 || python -c "import conductor" &> /dev/null 2>&1; then
        print_success "conductor-python package found"
        conductor_found=true
    elif [ -d "conductor-python-env" ]; then
        if source conductor-python-env/bin/activate && python -c "import conductor" &> /dev/null 2>&1; then
            print_success "conductor-python package found (virtual environment)"
            conductor_found=true
        fi
    fi
    
    if [ "$conductor_found" = false ]; then
        print_warning "conductor-python package not found"
        return 1
    fi
    
    if dotnet build SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj --no-restore &> /dev/null; then
        print_success "Python SDK builds successfully"
        return 0
    else
        print_error "Python SDK build failed"
        return 1
    fi
}

test_go_sdk() {
    print_status "Testing Go SDK..."
    if ! check_go; then
        print_warning "Go SDK test skipped - Go not installed"
        return 1
    fi
    
    local go_dir="SdkTestAutomation.Sdk/Implementations/Go"
    local original_dir=$(pwd)
    
    if [ ! -f "$go_dir/go-src/go.mod" ]; then
        print_error "go.mod file not found"
        return 1
    fi
    
    cd "$go_dir/go-src"
    if go list -m github.com/conductor-sdk/conductor-go &> /dev/null; then
        print_success "conductor-go SDK dependency found"
    else
        print_error "conductor-go SDK dependency missing"
        cd "$original_dir"
        return 1
    fi
    
    if go build -o /dev/null conductor-go-bridge.go 2>/dev/null; then
        print_success "Go code compiles successfully"
    else
        print_error "Go code compilation failed"
        cd "$original_dir"
        return 1
    fi
    
    cd "$original_dir"
    
    if check_shared_library; then
        print_success "Go shared library found"
    else
        print_warning "Go shared library not found"
    fi
    
    if dotnet build SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj --no-restore &> /dev/null; then
        print_success "Go SDK .NET integration builds successfully"
        return 0
    else
        print_error "Go SDK .NET integration build failed"
        return 1
    fi
}

# Create environment file
create_env_file() {
    print_status "Creating environment configuration..."
    
    local env_file="SdkTestAutomation.Tests/.env"
    mkdir -p "$(dirname "$env_file")"
    
    if [ ! -f "$env_file" ]; then
        cat > "$env_file" << EOF
# Conductor Server Configuration
CONDUCTOR_SERVER_URL=\${CONDUCTOR_SERVER_URL:-http://localhost:8080/api}

# SDK Selection (csharp, java, python, go)
SDK_TYPE=csharp

# Python Configuration
PYTHON_HOME=$(which python3 2>/dev/null || which python 2>/dev/null || echo "/usr/local/bin/python3")
PYTHONPATH=$(python3 -c "import site; print(site.getsitepackages()[0])" 2>/dev/null || echo "/usr/local/lib/python3.9/site-packages")

# Java Configuration
JAVA_HOME=\${JAVA_HOME:-$(echo $JAVA_HOME)}
JAVA_CLASSPATH=\${JAVA_CLASSPATH:-""}

# Go Configuration
GOPATH=\${GOPATH:-$(go env GOPATH 2>/dev/null || echo "$HOME/go")}
GOROOT=\${GOROOT:-$(go env GOROOT 2>/dev/null || echo "/usr/local/go")}
EOF
        print_success "Environment file created: $env_file"
    else
        print_warning "Environment file already exists: $env_file"
    fi
}

# Test all SDKs
test_all_sdks() {
    print_status "Testing all SDKs..."
    test_csharp_sdk
    test_java_sdk
    test_python_sdk
    test_go_sdk
}

# Build all projects
build_all_projects() {
    print_status "Building all projects..."
    if dotnet build SdkTestAutomation.sln; then
        print_success "All projects build successfully"
        return 0
    else
        print_error "Build failed"
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

# Rebuild Java CLI applications
rebuild_java_cli() {
    print_status "Rebuilding Java CLI applications..."
    
    if ! check_java; then
        print_error "Java not installed. Cannot rebuild Java CLI applications."
        return 1
    fi
    
    if ! command -v mvn &> /dev/null; then
        print_error "Maven not installed. Cannot rebuild Java CLI applications."
        return 1
    fi
    
    local java_cli_dir="SdkTestAutomation.Sdk/Implementations/Java/cli-java-sdk"
    local jar_dir="SdkTestAutomation.Sdk/bin/Debug/net8.0/lib"
    local original_dir=$(pwd)
    
    if [ ! -d "$java_cli_dir" ]; then
        print_error "Java CLI directory not found: $java_cli_dir"
        return 1
    fi
    
    cd "$java_cli_dir"
    
    # Make build script executable
    chmod +x build.sh
    
    # Clean and rebuild
    print_status "Cleaning previous builds..."
    mvn clean
    
    print_status "Building Java CLI applications..."
    if ./build.sh; then
        print_success "Java CLI applications rebuilt successfully"
        
        # Return to original directory for verification
        cd "$original_dir"
        
        # Verify JAR files
        if [ -f "$jar_dir/conductor-client.jar" ] && [ -f "$jar_dir/orkes-conductor-client.jar" ]; then
            print_success "JAR files verified:"
            print_status "  • conductor-client.jar ($(ls -lh "$jar_dir/conductor-client.jar" | awk '{print $5}'))"
            print_status "  • orkes-conductor-client.jar ($(ls -lh "$jar_dir/orkes-conductor-client.jar" | awk '{print $5}'))"
        else
            print_error "JAR files not found after rebuild"
            print_status "Expected files:"
            print_status "  • $jar_dir/conductor-client.jar"
            print_status "  • $jar_dir/orkes-conductor-client.jar"
            return 1
        fi
    else
        print_error "Failed to rebuild Java CLI applications"
        cd "$original_dir"
        return 1
    fi
    
    return 0
}

# Show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --setup-only      Setup SDKs (default)"
    echo "  --test-only       Test existing SDKs"
    echo "  --build-only      Build Go shared library only"
    echo "  --rebuild-java    Rebuild Java CLI applications only"
    echo "  --full            Setup, test, and build everything"
    echo "  --help            Show this help message"
}

# Parse arguments
MODE="setup"
while [[ $# -gt 0 ]]; do
    case $1 in
        --setup-only) MODE="setup"; shift ;;
        --test-only) MODE="test"; shift ;;
        --build-only) MODE="build"; shift ;;
        --rebuild-java) MODE="rebuild_java"; shift ;;
        --full) MODE="full"; shift ;;
        --help) show_usage; exit 0 ;;
        *) print_error "Unknown option: $1"; show_usage; exit 1 ;;
    esac
done

# Main execution
main() {
    case "$MODE" in
        "test")
            print_status "Running SDK tests..."
            test_all_sdks
            print_success "✅ SDK tests completed!"
            ;;
            
        "build")
            print_status "Building Go shared library..."
            if build_go_library; then
                print_success "✅ Go shared library build completed!"
            else
                print_error "❌ Go shared library build failed"
                exit 1
            fi
            ;;
            
        "rebuild_java")
            print_status "Rebuilding Java CLI applications..."
            if rebuild_java_cli; then
                print_success "✅ Java CLI applications rebuilt successfully!"
            else
                print_error "❌ Failed to rebuild Java CLI applications"
                exit 1
            fi
            ;;
            
        "full")
            print_status "Running full setup, test, and build..."
            check_dotnet
            
            setup_csharp_sdk
            if setup_java_sdk; then JAVA_READY=true; else JAVA_READY=false; fi
            if setup_python_sdk; then PYTHON_READY=true; else PYTHON_READY=false; fi
            if setup_go_sdk; then GO_READY=true; else GO_READY=false; fi
            
            create_env_file
            build_all_projects
            test_all_sdks
            
            if check_shared_library; then GO_SHARED_LIBRARY=true; else GO_SHARED_LIBRARY=false; fi
            print_summary "Full Setup Summary" "$JAVA_READY" "$PYTHON_READY" "$GO_READY" "$GO_SHARED_LIBRARY"
            ;;
            
        *)
            print_status "Starting SDK setup..."
            check_dotnet
            
            setup_csharp_sdk
            if setup_java_sdk; then JAVA_READY=true; else JAVA_READY=false; fi
            if setup_python_sdk; then PYTHON_READY=true; else PYTHON_READY=false; fi
            if setup_go_sdk; then GO_READY=true; else GO_READY=false; fi
            
            create_env_file
            build_all_projects
            
            if check_shared_library; then GO_SHARED_LIBRARY=true; else GO_SHARED_LIBRARY=false; fi
            print_summary "Setup Summary" "$JAVA_READY" "$PYTHON_READY" "$GO_READY" "$GO_SHARED_LIBRARY"
            ;;
    esac
    
    echo ""
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Update SdkTestAutomation.Tests/.env with your server URL"
    echo "3. Run tests with different SDKs:"
    echo "   • C# SDK: SDK_TYPE=csharp ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo "   • Java SDK: SDK_TYPE=java ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo "   • Python SDK: SDK_TYPE=python ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo "   • Go SDK: SDK_TYPE=go ./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo ""
    print_success "Operation complete! 🎉"
}

main "$@" 