#!/bin/bash

# SdkTestAutomation C# SDK Setup Script
set -e

# Get the directory where this script is located (project root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "SdkTestAutomation C# SDK Setup Script"
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

# Setup C# SDK
setup_csharp_sdk() {
    print_status "Setting up C# SDK..."
    if dotnet restore "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj"; then
        print_success "C# SDK setup complete"
        return 0
    else
        print_error "C# SDK setup failed"
        return 1
    fi
}

# Verify C# SDK
verify_csharp_sdk() {
    print_status "Verifying C# SDK..."
    if dotnet build "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj" --no-restore &> /dev/null; then
        print_success "C# SDK builds successfully"
        return 0
    else
        print_error "C# SDK build failed"
        return 1
    fi
}

# Show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --setup-only      Setup C# SDK (default)"
    echo "  --verify-only     Verify existing C# SDK"
    echo "  --full            Setup and verify C# SDK"
    echo "  --help            Show this help message"
}

# Parse arguments
MODE="setup"
while [[ $# -gt 0 ]]; do
    case $1 in
        --setup-only) MODE="setup"; shift ;;
        --verify-only) MODE="verify"; shift ;;
        --full) MODE="full"; shift ;;
        --help) show_usage; exit 0 ;;
        *) print_error "Unknown option: $1"; show_usage; exit 1 ;;
    esac
done

# Main execution
main() {
    case "$MODE" in
        "verify")
            print_status "Verifying C# SDK..."
            verify_csharp_sdk
            print_success "✅ C# SDK verification completed!"
            ;;
            
        "full")
            print_status "Running full C# SDK setup and verify..."
            check_dotnet
            setup_csharp_sdk
            verify_csharp_sdk
            ;;
            
        *)
            print_status "Starting C# SDK setup..."
            check_dotnet
            setup_csharp_sdk
            ;;
    esac
    
    echo ""
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Run tests with C# SDK:"
    echo "   • SDK_TYPE=csharp $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo ""
    print_success "C# SDK setup complete! 🎉"
}

main "$@" 