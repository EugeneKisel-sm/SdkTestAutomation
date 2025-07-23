#!/bin/bash

# SdkTestAutomation - Multi-SDK Test Runner
# 
# This script runs tests against multiple Conductor SDKs (C#, Java, Python)
# by building the necessary CLI wrappers and executing the .NET test framework.
#
# Usage:
#   ./run-tests.sh                    # Run tests with all SDKs
#   ./run-tests.sh csharp             # Run tests with C# SDK only
#   ./run-tests.sh java               # Run tests with Java SDK only
#   ./run-tests.sh python             # Run tests with Python SDK only
#   ./run-tests.sh --help             # Show this help message
#   ./run-tests.sh --validate         # Validate environment and dependencies

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
TEST_EXECUTABLE="./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
TEST_FILTER="/SdkTestAutomation.Tests/SdkTestAutomation.Tests.Conductor.EventResource/SdkIntegrationTests"
CONDUCTOR_URL="${CONDUCTOR_SERVER_URL:-http://localhost:8080/api}"

# Functions
print_header() {
    echo -e "${BLUE}=== SdkTestAutomation Multi-SDK Test Runner ===${NC}"
    echo ""
}

print_help() {
    cat << EOF
SdkTestAutomation Multi-SDK Test Runner

This script runs tests against multiple Conductor SDKs by building the necessary
CLI wrappers and executing the .NET test framework.

Usage:
  $0                    # Run tests with all SDKs
  $0 csharp             # Run tests with C# SDK only
  $0 java               # Run tests with Java SDK only
  $0 python             # Run tests with Python SDK only
  $0 --help             # Show this help message
  $0 --validate         # Validate environment and dependencies

Environment Variables:
  CONDUCTOR_SERVER_URL  Conductor server URL (default: http://localhost:8080/api)
  SDK_TYPE              SDK to test (csharp, java, python)

Prerequisites:
  - .NET 8.0 SDK
  - Java 17+ and Maven (for Java SDK)
  - Python 3.9+ and pip (for Python SDK)
  - Conductor server running at CONDUCTOR_SERVER_URL

Examples:
  CONDUCTOR_SERVER_URL=http://localhost:8080/api $0
  $0 csharp
  $0 --validate
EOF
}

validate_environment() {
    echo -e "${BLUE}Validating environment...${NC}"
    
    # Check .NET
    if ! command -v dotnet &> /dev/null; then
        echo -e "${RED}❌ .NET SDK not found. Please install .NET 8.0 SDK.${NC}"
        return 1
    fi
    
    local dotnet_version=$(dotnet --version)
    echo -e "${GREEN}✅ .NET SDK found: $dotnet_version${NC}"
    
    # Check test executable
    if [ ! -f "$TEST_EXECUTABLE" ]; then
        echo -e "${YELLOW}⚠️  Test executable not found. Building tests...${NC}"
        build_tests
    else
        echo -e "${GREEN}✅ Test executable found${NC}"
    fi
    
    # Check Java (optional)
    if command -v java &> /dev/null && command -v mvn &> /dev/null; then
        local java_version=$(java -version 2>&1 | head -n 1)
        local mvn_version=$(mvn --version 2>/dev/null | head -n 1 | cut -d' ' -f3)
        echo -e "${GREEN}✅ Java found: $java_version${NC}"
        echo -e "${GREEN}✅ Maven found: $mvn_version${NC}"
    else
        echo -e "${YELLOW}⚠️  Java or Maven not found - Java SDK tests will be skipped${NC}"
    fi
    
    # Check Python (optional)
    if command -v python3 &> /dev/null; then
        local python_version=$(python3 --version)
        echo -e "${GREEN}✅ Python found: $python_version${NC}"
    else
        echo -e "${YELLOW}⚠️  Python3 not found - Python SDK tests will be skipped${NC}"
    fi
    
    # Check Conductor server
    if curl -s "$CONDUCTOR_URL" > /dev/null 2>&1; then
        echo -e "${GREEN}✅ Conductor server is accessible at $CONDUCTOR_URL${NC}"
    else
        echo -e "${RED}❌ Conductor server is not accessible at $CONDUCTOR_URL${NC}"
        echo -e "${YELLOW}   Make sure the server is running: docker run -d -p 8080:8080 conductoross/conductor-server:latest${NC}"
        return 1
    fi
    
    echo -e "${GREEN}✅ Environment validation completed${NC}"
    return 0
}

build_tests() {
    echo -e "${BLUE}Building .NET tests...${NC}"
    dotnet build SdkTestAutomation.Tests/SdkTestAutomation.Tests.csproj -c Debug
    echo -e "${GREEN}✅ Tests built successfully${NC}"
}

run_tests_with_sdk() {
    local sdk_type=$1
    echo -e "${BLUE}Running tests with $sdk_type SDK...${NC}"
    
    # Set environment variables
    export SDK_TYPE=$sdk_type
    export CONDUCTOR_SERVER_URL=$CONDUCTOR_URL
    
    # Run tests
    if $TEST_EXECUTABLE -filter "$TEST_FILTER"; then
        echo -e "${GREEN}✅ $sdk_type SDK tests completed successfully${NC}"
        return 0
    else
        echo -e "${RED}❌ $sdk_type SDK tests failed${NC}"
        return 1
    fi
}

setup_java_wrapper() {
    if command -v java &> /dev/null && command -v mvn &> /dev/null; then
        echo -e "${BLUE}Building Java wrapper...${NC}"
        cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Java
        if mvn clean package -q; then
            echo -e "${GREEN}✅ Java wrapper built successfully${NC}"
            cd ../../..
            return 0
        else
            echo -e "${RED}❌ Failed to build Java wrapper${NC}"
            cd ../../..
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Skipping Java SDK - Java or Maven not found${NC}"
        return 1
    fi
}

setup_python_wrapper() {
    if command -v python3 &> /dev/null; then
        echo -e "${BLUE}Setting up Python wrapper...${NC}"
        cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Python
        
        # Create virtual environment if it doesn't exist
        if [ ! -d "venv" ]; then
            echo -e "${BLUE}Creating Python virtual environment...${NC}"
            python3 -m venv venv
        fi
        
        # Install package
        if source venv/bin/activate && pip install -e . -q; then
            echo -e "${GREEN}✅ Python wrapper installed successfully${NC}"
            cd ../../..
            return 0
        else
            echo -e "${RED}❌ Failed to install Python wrapper${NC}"
            cd ../../..
            return 1
        fi
    else
        echo -e "${YELLOW}⚠️  Skipping Python SDK - Python3 not found${NC}"
        return 1
    fi
}

cleanup() {
    echo -e "${BLUE}Cleaning up...${NC}"
    # Add any cleanup tasks here if needed
    echo -e "${GREEN}✅ Cleanup completed${NC}"
}

# Main execution
main() {
    print_header
    
    # Handle help and validation flags
    case "${1:-}" in
        --help|-h)
            print_help
            exit 0
            ;;
        --validate)
            validate_environment
            exit $?
            ;;
    esac
    
    # Validate environment before running tests
    if ! validate_environment; then
        echo -e "${RED}❌ Environment validation failed. Please fix the issues above.${NC}"
        exit 1
    fi
    
    # Run specific SDK or all SDKs
    if [ $# -eq 1 ]; then
        case $1 in
            "csharp"|"java"|"python")
                if [ "$1" = "java" ] && ! setup_java_wrapper; then
                    echo -e "${RED}❌ Failed to setup Java wrapper${NC}"
                    exit 1
                elif [ "$1" = "python" ] && ! setup_python_wrapper; then
                    echo -e "${RED}❌ Failed to setup Python wrapper${NC}"
                    exit 1
                fi
                run_tests_with_sdk $1
                ;;
            *)
                echo -e "${RED}❌ Invalid SDK type: $1${NC}"
                echo "Usage: $0 [csharp|java|python|--help|--validate]"
                exit 1
                ;;
        esac
    else
        # Run all SDKs
        echo -e "${BLUE}Running tests with all supported SDKs...${NC}"
        echo ""
        
        local failed_sdks=()
        
        # C# SDK (always available)
        if run_tests_with_sdk "csharp"; then
            echo -e "${GREEN}✅ C# SDK tests passed${NC}"
        else
            failed_sdks+=("csharp")
        fi
        echo ""
        
        # Java SDK
        if setup_java_wrapper; then
            if run_tests_with_sdk "java"; then
                echo -e "${GREEN}✅ Java SDK tests passed${NC}"
            else
                failed_sdks+=("java")
            fi
        fi
        echo ""
        
        # Python SDK
        if setup_python_wrapper; then
            if run_tests_with_sdk "python"; then
                echo -e "${GREEN}✅ Python SDK tests passed${NC}"
            else
                failed_sdks+=("python")
            fi
        fi
        echo ""
        
        # Summary
        if [ ${#failed_sdks[@]} -eq 0 ]; then
            echo -e "${GREEN}🎉 All SDK tests completed successfully!${NC}"
        else
            echo -e "${RED}❌ Some SDK tests failed: ${failed_sdks[*]}${NC}"
            exit 1
        fi
    fi
    
    cleanup
}

# Run main function with all arguments
main "$@" 