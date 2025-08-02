#!/bin/bash

# SdkTestAutomation Python SDK Setup Script
set -e

# Get the directory where this script is located (project root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "SdkTestAutomation Python SDK Setup Script"
echo "============================================"

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
check_python() {
    # First check for python3.11 specifically
    if command -v python3.11 &> /dev/null; then
        local version=$(python3.11 --version | cut -d' ' -f2)
        if check_version "$version" "3.11.0"; then
            print_success "Python $version is installed (python3.11)"
            return 0
        else
            print_warning "Python $version is installed but version 3.11+ is required"
            return 1
        fi
    elif command -v python3 &> /dev/null; then
        local version=$(python3 --version | cut -d' ' -f2)
        if check_version "$version" "3.11.0"; then
            print_success "Python $version is installed"
            return 0
        else
            print_warning "Python $version is installed but version 3.11+ is required"
            return 1
        fi
    elif command -v python &> /dev/null; then
        local version=$(python --version | cut -d' ' -f2)
        if check_version "$version" "3.11.0"; then
            print_success "Python $version is installed"
            return 0
        else
            print_warning "Python $version is installed but version 3.11+ is required"
            return 1
        fi
    else
        print_warning "Python is not installed. Please install Python 3.11+ for Python SDK support."
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

# Setup Python SDK
setup_python_sdk() {
    print_status "Setting up Python SDK..."
    
    if ! check_python; then
        return 1
    fi
    
    # Try to install conductor-python
    local installed=false
    for cmd in "python3.11 -m pip install conductor-python" "pip3.11 install conductor-python" "pip3 install conductor-python" "pip install conductor-python" "python3 -m pip install conductor-python"; do
        if eval "$cmd" &> /dev/null; then
            print_success "conductor-python package installed"
            installed=true
            break
        fi
    done
    
    if [ "$installed" = false ]; then
        print_warning "Failed to install conductor-python via pip"
        # Try to create virtual environment with python3.11 first, then fallback to python3
        local venv_created=false
        for python_cmd in "python3.11" "python3"; do
            if command -v "$python_cmd" -m venv &> /dev/null; then
                print_status "Creating virtual environment with $python_cmd..."
                "$python_cmd" -m venv conductor-python-env
                source conductor-python-env/bin/activate
                if pip install conductor-python &> /dev/null; then
                    print_success "conductor-python installed in virtual environment"
                    print_status "Activate with: source conductor-python-env/bin/activate"
                    installed=true
                    venv_created=true
                    break
                fi
            fi
        done
        
        if [ "$venv_created" = false ]; then
            print_warning "Could not create virtual environment"
        fi
    fi
    
    if [ "$installed" = false ]; then
        print_warning "Could not install conductor-python"
        return 1
    fi
    
    if dotnet restore "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj"; then
        print_success "Python SDK setup complete"
        return 0
    else
        print_error "Python SDK setup failed"
        return 1
    fi
}

# Verify Python SDK
verify_python_sdk() {
    print_status "Verifying Python SDK..."
    if ! check_python; then
        print_warning "Python SDK test skipped - Python not installed"
        return 1
    fi
    
    local conductor_found=false
    if python3.11 -c "import conductor" &> /dev/null 2>&1 || python3 -c "import conductor" &> /dev/null 2>&1 || python -c "import conductor" &> /dev/null 2>&1; then
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
    
    if dotnet build "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj" --no-restore &> /dev/null; then
        print_success "Python SDK builds successfully"
        return 0
    else
        print_error "Python SDK build failed"
        return 1
    fi
}

# Show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --setup-only      Setup Python SDK (default)"
    echo "  --verify-only     Verify existing Python SDK"
    echo "  --full            Setup and verify Python SDK"
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
            print_status "Verifying Python SDK..."
            verify_python_sdk
            print_success "✅ Python SDK verification completed!"
            ;;
            
        "full")
            print_status "Running full Python SDK setup and verify..."
            check_dotnet
            setup_python_sdk
            verify_python_sdk
            ;;
            
        *)
            print_status "Starting Python SDK setup..."
            check_dotnet
            setup_python_sdk
            ;;
    esac
    
    echo ""
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Run tests with Python SDK:"
    echo "   • SDK_TYPE=python $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo ""
    print_success "Python SDK setup complete! 🎉"
}

main "$@" 