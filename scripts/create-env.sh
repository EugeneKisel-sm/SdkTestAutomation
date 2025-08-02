#!/bin/bash

# SdkTestAutomation Environment File Creation Script
set -e

# Get the directory where this script is located (project root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "SdkTestAutomation Environment File Creation Script"
echo "===================================================="

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

# Create environment file for specific SDK
create_env_file() {
    local sdk_type="$1"
    local server_url="${2:-http://localhost:8080/api}"
    
    print_status "Creating environment configuration for $sdk_type SDK..."
    
    local env_file="$SCRIPT_DIR/SdkTestAutomation.Tests/.env"
    mkdir -p "$(dirname "$env_file")"
    
    # Base configuration
    cat > "$env_file" << EOF
# Conductor Server Configuration
CONDUCTOR_SERVER_URL=$server_url

# SDK Selection (csharp, java, python, go)
SDK_TYPE=$sdk_type
EOF

    # Add SDK-specific configuration
    case "$sdk_type" in
        "java")
            cat >> "$env_file" << EOF

# Java Configuration
JAVA_HOME=\${JAVA_HOME:-$(echo $JAVA_HOME)}
JAVA_CLASSPATH=\${JAVA_CLASSPATH:-""}
EOF
            ;;
        "python")
            cat >> "$env_file" << EOF

# Python Configuration
PYTHON_HOME=$(which python3.11 2>/dev/null || which python3 2>/dev/null || which python 2>/dev/null || echo "/usr/local/bin/python3.11")
PYTHONPATH=$(python3.11 -c "import site; print(site.getsitepackages()[0])" 2>/dev/null || python3 -c "import site; print(site.getsitepackages()[0])" 2>/dev/null || echo "/usr/local/lib/python3.11/site-packages")
EOF
            ;;
        "go")
            cat >> "$env_file" << EOF

# Go Configuration
GOPATH=\${GOPATH:-$(go env GOPATH 2>/dev/null || echo "$HOME/go")}
GOROOT=\${GOROOT:-$(go env GOROOT 2>/dev/null || echo "/usr/local/go")}
EOF
            ;;
    esac
    
    print_success "Environment file created: $env_file"
    print_status "SDK_TYPE set to: $sdk_type"
    print_status "CONDUCTOR_SERVER_URL set to: $server_url"
}

# Show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --sdk TYPE        SDK type (csharp, java, python, go) [default: csharp]"
    echo "  --server URL      Conductor server URL [default: http://localhost:8080/api]"
    echo "  --help            Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 --sdk csharp"
    echo "  $0 --sdk java --server http://my-server:8080/api"
    echo "  $0 --sdk python"
    echo "  $0 --sdk go"
}

# Parse arguments
SDK_TYPE="csharp"
SERVER_URL="http://localhost:8080/api"

while [[ $# -gt 0 ]]; do
    case $1 in
        --sdk)
            SDK_TYPE="$2"
            shift 2
            ;;
        --server)
            SERVER_URL="$2"
            shift 2
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

# Validate SDK type
case "$SDK_TYPE" in
    "csharp"|"java"|"python"|"go")
        ;;
    *)
        print_error "Invalid SDK type: $SDK_TYPE"
        print_status "Valid SDK types: csharp, java, python, go"
        exit 1
        ;;
esac

# Main execution
main() {
    print_status "Creating environment file..."
    create_env_file "$SDK_TYPE" "$SERVER_URL"
    
    echo ""
    print_status "Environment file created successfully!"
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Run tests with $SDK_TYPE SDK:"
    echo "   • $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo ""
    print_success "Environment setup complete! 🎉"
}

main "$@" 