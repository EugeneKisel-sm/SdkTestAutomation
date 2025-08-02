#!/bin/bash

# SdkTestAutomation Java SDK Setup Script
set -e

# Get the directory where this script is located (project root)
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

echo "SdkTestAutomation Java SDK Setup Script"
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

# Version check
check_version() {
    local version=$1
    local required=$2
    [ "$(printf '%s\n' "$required" "$version" | sort -V | head -n1)" = "$required" ]
}

# Prerequisites check
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

# Setup Java SDK
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
    
    local java_cli_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/Implementations/Java/cli-java-sdk"
    local jar_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/bin/Debug/net8.0/lib"
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
    
    if dotnet restore "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj"; then
        print_success "Java SDK setup complete"
        return 0
    else
        print_error "Java SDK setup failed"
        return 1
    fi
}

# Verify Java SDK
verify_java_sdk() {
    print_status "Verifying Java SDK..."
    if ! check_java; then
        print_warning "Java SDK test skipped - Java not installed"
        return 1
    fi
    
    local jar_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/bin/Debug/net8.0/lib"
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
    
    if dotnet build "$SCRIPT_DIR/SdkTestAutomation.Sdk/SdkTestAutomation.Sdk.csproj" --no-restore &> /dev/null; then
        print_success "Java SDK builds successfully"
        return 0
    else
        print_error "Java SDK build failed"
        return 1
    fi
}

# Build Java CLI applications only
build_java_cli() {
    print_status "Building Java CLI applications..."
    
    if ! check_java; then
        print_error "Java is not installed. Cannot build Java CLI applications."
        return 1
    fi
    
    # Check if Maven is installed
    if ! command -v mvn &> /dev/null; then
        print_error "Maven is not installed. Please install Maven for Java SDK support."
        print_status "Install with: brew install maven (macOS) or download from maven.apache.org"
        return 1
    fi
    
    local java_cli_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/Implementations/Java/cli-java-sdk"
    local jar_dir="$SCRIPT_DIR/SdkTestAutomation.Sdk/bin/Debug/net8.0/lib"
    local original_dir=$(pwd)
    
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
        
        # Verify JAR files were created
        if [ -f "$jar_dir/conductor-client.jar" ] && [ -f "$jar_dir/orkes-conductor-client.jar" ]; then
            print_success "Java CLI JAR files verified:"
            print_status "  • conductor-client.jar ($(ls -lh "$jar_dir/conductor-client.jar" | awk '{print $5}'))"
            print_status "  • orkes-conductor-client.jar ($(ls -lh "$jar_dir/orkes-conductor-client.jar" | awk '{print $5}'))"
            return 0
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
}

# Show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --setup-only      Setup Java SDK (default)"
    echo "  --verify-only     Verify existing Java SDK"
    echo "  --build-only      Build Java CLI applications only"
    echo "  --full            Setup and verify Java SDK"
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
            print_status "Verifying Java SDK..."
            verify_java_sdk
            print_success "✅ Java SDK verification completed!"
            ;;
            
        "build")
            print_status "Building Java CLI applications..."
            if build_java_cli; then
                print_success "✅ Java CLI applications build completed!"
            else
                print_error "❌ Java CLI applications build failed"
                exit 1
            fi
            ;;
            
        "full")
            print_status "Running full Java SDK setup and verify..."
            check_dotnet
            setup_java_sdk
            verify_java_sdk
            ;;
            
        *)
            print_status "Starting Java SDK setup..."
            check_dotnet
            setup_java_sdk
            ;;
    esac
    
    echo ""
    print_status "Next steps:"
    echo "1. Start your Conductor server"
    echo "2. Run tests with Java SDK:"
    echo "   • SDK_TYPE=java $SCRIPT_DIR/SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
    echo ""
    print_success "Java SDK setup complete! 🎉"
}

main "$@" 