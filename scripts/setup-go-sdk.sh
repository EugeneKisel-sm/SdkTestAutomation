#!/bin/bash

# SdkTestAutomation Go SDK Setup Script
set -e

# Get the directory where this script is located (project root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "SdkTestAutomation Go SDK Setup Script"
echo "========================================"

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
    local go_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/Implementations/Go"
    local build_artifacts_dir="$go_dir/build-artifacts"
    
    # Check for any shared library file (the build script handles platform detection)
    if [ -d "$build_artifacts_dir" ] && [ "$(ls -A "$build_artifacts_dir" 2>/dev/null | grep -E '\.(dylib|so|dll)$')" ]; then
        local library_file=$(ls "$build_artifacts_dir"/*.dylib "$build_artifacts_dir"/*.so "$build_artifacts_dir"/*.dll 2>/dev/null | head -1)
        print_success "Shared library found: $library_file"
        return 0
    else
        print_warning "Shared library not found in: $build_artifacts_dir"
        return 1
    fi
}

# Clean up Go artifacts
cleanup_go_artifacts() {
    local go_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/Implementations/Go"
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
    local go_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/Implementations/Go"
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
    
    # Make build script executable and run it
    chmod +x build.sh
    print_status "Building Go shared library using build.sh..."
    if ./build.sh; then
        print_success "Successfully built Go shared library"
        cd "$original_dir"
        return 0
    else
        print_error "Failed to build Go shared library"
        cd "$original_dir"
        return 1
    fi
}

# Setup Go SDK
setup_go_sdk() {
    print_status "Setting up Go SDK..."
    
    cleanup_go_artifacts
    
    if ! check_go; then
        return 1
    fi
    
    if build_go_library; then
        if dotnet restore "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj" && \
           dotnet build "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj" --no-restore &> /dev/null; then
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

# Verify Go SDK
verify_go_sdk() {
    print_status "Verifying Go SDK..."
    if ! check_go; then
        print_warning "Go SDK test skipped - Go not installed"
        return 1
    fi
    
    local go_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/Implementations/Go"
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
    
    if dotnet build "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj" --no-restore &> /dev/null; then
        print_success "Go SDK .NET integration builds successfully"
        return 0
    else
        print_error "Go SDK .NET integration build failed"
        return 1
    fi
}

# Show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --setup-only      Setup Go SDK (default)"
    echo "  --verify-only     Verify existing Go SDK"
    echo "  --build-only      Build Go shared library only"
    echo "  --full            Setup and verify Go SDK"
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
            print_status "Verifying Go SDK..."
            verify_go_sdk
            print_success "✅ Go SDK verification completed!"
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
            
        "full")
            print_status "Running full Go SDK setup and verify..."
            check_dotnet
            setup_go_sdk
            verify_go_sdk
            ;;
            
        *)
            print_status "Starting Go SDK setup..."
            check_dotnet
            setup_go_sdk
            ;;
    esac
    
    echo ""
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Run tests with Go SDK:"
    echo "   • SDK_TYPE=go $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo ""
    print_success "Go SDK setup complete! 🎉"
}

main "$@" 