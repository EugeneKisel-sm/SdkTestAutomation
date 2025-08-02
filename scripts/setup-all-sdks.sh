#!/bin/bash

# SdkTestAutomation All SDKs Setup Script
set -e

# Get the directory where this script is located (project root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "SdkTestAutomation All SDKs Setup Script"
echo "=========================================="

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

# Prerequisites check
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

# Check if individual setup scripts exist
check_setup_scripts() {
    local scripts=("setup-csharp-sdk.sh" "setup-java-sdk.sh" "setup-python-sdk.sh" "setup-go-sdk.sh" "create-env.sh")
    local missing_scripts=()
    
    for script in "${scripts[@]}"; do
        if [ ! -f "$SCRIPT_DIR/scripts/$script" ]; then
            missing_scripts+=("$script")
        fi
    done
    
    if [ ${#missing_scripts[@]} -gt 0 ]; then
        print_error "Missing setup scripts: ${missing_scripts[*]}"
        return 1
    fi
    
    print_success "All setup scripts found"
    return 0
}

# Setup individual SDKs using their respective scripts
setup_csharp_sdk() {
    print_status "Setting up C# SDK..."
    if "$SCRIPT_DIR/scripts/setup-csharp-sdk.sh" --setup-only; then
        print_success "C# SDK setup completed"
        return 0
    else
        print_error "C# SDK setup failed"
        return 1
    fi
}

setup_java_sdk() {
    print_status "Setting up Java SDK..."
    if "$SCRIPT_DIR/scripts/setup-java-sdk.sh" --setup-only; then
        print_success "Java SDK setup completed"
        return 0
    else
        print_error "Java SDK setup failed"
        return 1
    fi
}

setup_python_sdk() {
    print_status "Setting up Python SDK..."
    if "$SCRIPT_DIR/scripts/setup-python-sdk.sh" --setup-only; then
        print_success "Python SDK setup completed"
        return 0
    else
        print_error "Python SDK setup failed"
        return 1
    fi
}

setup_go_sdk() {
    print_status "Setting up Go SDK..."
    if "$SCRIPT_DIR/scripts/setup-go-sdk.sh" --setup-only; then
        print_success "Go SDK setup completed"
        return 0
    else
        print_error "Go SDK setup failed"
        return 1
    fi
}

# Verify individual SDKs using their respective scripts
verify_csharp_sdk() {
    print_status "Verifying C# SDK..."
    if "$SCRIPT_DIR/scripts/setup-csharp-sdk.sh" --verify-only; then
        print_success "C# SDK verification completed"
        return 0
    else
        print_error "C# SDK verification failed"
        return 1
    fi
}

verify_java_sdk() {
    print_status "Verifying Java SDK..."
    if "$SCRIPT_DIR/scripts/setup-java-sdk.sh" --verify-only; then
        print_success "Java SDK verification completed"
        return 0
    else
        print_error "Java SDK verification failed"
        return 1
    fi
}

verify_python_sdk() {
    print_status "Verifying Python SDK..."
    if "$SCRIPT_DIR/scripts/setup-python-sdk.sh" --verify-only; then
        print_success "Python SDK verification completed"
        return 0
    else
        print_error "Python SDK verification failed"
        return 1
    fi
}

verify_go_sdk() {
    print_status "Verifying Go SDK..."
    if "$SCRIPT_DIR/scripts/setup-go-sdk.sh" --verify-only; then
        print_success "Go SDK verification completed"
        return 0
    else
        print_error "Go SDK verification failed"
        return 1
    fi
}

# Build Go shared library only
build_go_library() {
    print_status "Building Go shared library..."
    if "$SCRIPT_DIR/scripts/setup-go-sdk.sh" --build-only; then
        print_success "Go shared library build completed"
        return 0
    else
        print_error "Go shared library build failed"
        return 1
    fi
}

# Build Java CLI applications only
build_java_cli() {
    print_status "Building Java CLI applications..."
    if "$SCRIPT_DIR/scripts/setup-java-sdk.sh" --build-only; then
        print_success "Java CLI applications build completed"
        return 0
    else
        print_error "Java CLI applications build failed"
        return 1
    fi
}

# Verify all SDKs
verify_all_sdks() {
    print_status "Verifying all SDKs..."
    verify_csharp_sdk
    verify_java_sdk
    verify_python_sdk
    verify_go_sdk
}

# Build all projects
build_all_projects() {
    print_status "Building all projects..."
    if dotnet build "$SCRIPT_DIR/SdkTestAutomation.sln"; then
        print_success "All projects build successfully"
        return 0
    else
        print_error "Build failed"
        return 1
    fi
}

# Create environment file
create_env_file() {
    print_status "Creating environment configuration..."
    if "$SCRIPT_DIR/scripts/create-env.sh" --sdk csharp; then
        print_success "Environment file created"
        return 0
    else
        print_error "Environment file creation failed"
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

# Show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Description:"
    echo "  Sets up all SDKs (C#, Java, Python, Go) for SdkTestAutomation framework"
    echo ""
    echo "Options:"
    echo "  --setup-only      Setup all SDKs (default)"
    echo "  --verify-only     Verify existing SDKs"
    echo "  --build-only      Build SDK artifacts (Go shared library and Java CLI)"
    echo "  --full            Setup, verify, and build everything"
    echo "  --help            Show this help message"
}

# Parse arguments
MODE="setup"
while [[ $# -gt 0 ]]; do
    case $1 in
        --setup-only) MODE="setup"; shift ;;
        --verify-only) MODE="verify"; shift ;;
        --build-only) MODE="build"; shift ;;
        --full) MODE="full"; shift ;;
        --help) show_usage; exit 0 ;;
        *) print_error "Unknown option: $1"; show_usage; exit 1 ;;
    esac
done

# Main execution
main() {
    case "$MODE" in
        "verify")
            print_status "Verifying SDKs..."
            verify_all_sdks
            print_success "✅ SDK verification completed!"
            ;;
            
        "build")
            print_status "Building SDK artifacts..."
            local build_success=true
            
            print_status "Building Go shared library..."
            if build_go_library; then
                print_success "✅ Go shared library build completed!"
            else
                print_error "❌ Go shared library build failed"
                build_success=false
            fi
            
            print_status "Building Java CLI applications..."
            if build_java_cli; then
                print_success "✅ Java CLI applications build completed!"
            else
                print_error "❌ Java CLI applications build failed"
                build_success=false
            fi
            
            if [ "$build_success" = true ]; then
                print_success "✅ All SDK artifacts build completed!"
            else
                print_error "❌ Some SDK artifacts build failed"
                exit 1
            fi
            ;;
            
        "full")
            print_status "Running full setup, verify, and build..."
            check_dotnet
            check_setup_scripts
            
            setup_csharp_sdk
            if setup_java_sdk; then JAVA_READY=true; else JAVA_READY=false; fi
            if setup_python_sdk; then PYTHON_READY=true; else PYTHON_READY=false; fi
            if setup_go_sdk; then GO_READY=true; else GO_READY=false; fi
            
            create_env_file
            build_all_projects
            verify_all_sdks
            
            # Check if Go shared library exists
            local go_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/Implementations/Go"
            local build_artifacts_dir="$go_dir/build-artifacts"
            if [ -d "$build_artifacts_dir" ] && [ "$(ls -A "$build_artifacts_dir" 2>/dev/null)" ]; then
                GO_SHARED_LIBRARY=true
            else
                GO_SHARED_LIBRARY=false
            fi
            
            print_summary "Full Setup Summary" "$JAVA_READY" "$PYTHON_READY" "$GO_READY" "$GO_SHARED_LIBRARY"
            ;;
            
        *)
            print_status "Starting SDK setup..."
            check_dotnet
            check_setup_scripts
            
            setup_csharp_sdk
            if setup_java_sdk; then JAVA_READY=true; else JAVA_READY=false; fi
            if setup_python_sdk; then PYTHON_READY=true; else PYTHON_READY=false; fi
            if setup_go_sdk; then GO_READY=true; else GO_READY=false; fi
            
            create_env_file
            build_all_projects
            
            # Check if Go shared library exists
            local go_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/Implementations/Go"
            local build_artifacts_dir="$go_dir/build-artifacts"
            if [ -d "$build_artifacts_dir" ] && [ "$(ls -A "$build_artifacts_dir" 2>/dev/null)" ]; then
                GO_SHARED_LIBRARY=true
            else
                GO_SHARED_LIBRARY=false
            fi
            
            print_summary "Setup Summary" "$JAVA_READY" "$PYTHON_READY" "$GO_READY" "$GO_SHARED_LIBRARY"
            ;;
    esac
    
    echo ""
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Update $SCRIPT_DIR/SdkTestAutomation.Tests/.env with your server URL"
    echo "3. Run tests with different SDKs:"
    echo "   • C# SDK: SDK_TYPE=csharp $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo "   • Java SDK: SDK_TYPE=java $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo "   • Python SDK: SDK_TYPE=python $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo "   • Go SDK: SDK_TYPE=go $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo ""
    print_success "Operation complete! 🎉"
}

main "$@" 