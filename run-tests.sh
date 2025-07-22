#!/bin/bash

# SdkTestAutomation - Multi-SDK Test Runner
echo "=== SdkTestAutomation Multi-SDK Test Runner ==="

TEST_EXECUTABLE="./SdkTestAutomation.Tests/bin/Debug/net8.0/SdkTestAutomation.Tests"
TEST_FILTER="/SdkTestAutomation.Tests/SdkTestAutomation.Tests.Conductor.EventResource/SdkIntegrationTests"

run_tests_with_sdk() {
    local sdk_type=$1
    echo "Running tests with $sdk_type SDK..."
    SDK_TYPE=$sdk_type $TEST_EXECUTABLE -filter "$TEST_FILTER"
    echo "=== $sdk_type SDK tests completed ==="
    echo ""
}

setup_java_wrapper() {
    if command -v java &> /dev/null && command -v mvn &> /dev/null; then
        echo "Building Java wrapper..."
        cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Java
        mvn clean package -q
        cd ../../..
        return 0
    else
        echo "Skipping Java SDK - Java or Maven not found"
        return 1
    fi
}

setup_python_wrapper() {
    if command -v python3 &> /dev/null; then
        echo "Setting up Python wrapper..."
        cd SdkTestAutomation.CliWrappers/SdkTestAutomation.Python
        if [ ! -d "venv" ]; then
            python3 -m venv venv
        fi
        source venv/bin/activate && pip install -e . -q
        cd ../../..
        return 0
    else
        echo "Skipping Python SDK - Python3 not found"
        return 1
    fi
}

# Main execution
if [ $# -eq 1 ]; then
    case $1 in
        "csharp"|"java"|"python")
            if [ "$1" = "java" ] && ! setup_java_wrapper; then
                exit 1
            elif [ "$1" = "python" ] && ! setup_python_wrapper; then
                exit 1
            fi
            run_tests_with_sdk $1
            ;;
        *)
            echo "Usage: $0 [csharp|java|python]"
            echo "If no SDK is specified, all SDKs will be tested."
            exit 1
            ;;
    esac
else
    # Run all SDKs
    echo "Running tests with all supported SDKs..."
    echo ""
    
    run_tests_with_sdk "csharp"
    
    if setup_java_wrapper; then
        run_tests_with_sdk "java"
    fi
    
    if setup_python_wrapper; then
        run_tests_with_sdk "python"
    fi
    
    echo "=== All SDK tests completed ==="
fi

echo ""
echo "Note: Make sure Conductor server is running at http://localhost:8080"
echo "To run a specific SDK test: $0 [csharp|java|python]" 