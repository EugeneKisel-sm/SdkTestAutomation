#!/bin/bash

# SdkTestAutomation - CLI Wrappers Build Script
# Builds C#, Java, and Python CLI wrappers for testing

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Help function
show_help() {
    echo "SdkTestAutomation CLI Wrappers Build Script"
    echo ""
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -h, --help          Show this help message"
    echo "  -a, --all           Build all wrappers (default)"
    echo "  -c, --csharp        Build only C# wrapper"
    echo "  -j, --java          Build only Java wrapper"
    echo "  -p, --python        Build only Python wrapper"
    echo "  -v, --verbose       Enable verbose output"
    echo "  --clean             Clean build artifacts before building"
    echo ""
    echo "Examples:"
    echo "  $0                  # Build all wrappers"
    echo "  $0 --csharp         # Build only C# wrapper"
    echo "  $0 --java --python  # Build Java and Python wrappers"
    echo "  $0 --clean          # Clean and build all wrappers"
}

# Variables
BUILD_ALL=true
BUILD_CSHARP=false
BUILD_JAVA=false
BUILD_PYTHON=false
VERBOSE=false
CLEAN=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -h|--help)
            show_help
            exit 0
            ;;
        -a|--all)
            BUILD_ALL=true
            shift
            ;;
        -c|--csharp)
            BUILD_ALL=false
            BUILD_CSHARP=true
            shift
            ;;
        -j|--java)
            BUILD_ALL=false
            BUILD_JAVA=true
            shift
            ;;
        -p|--python)
            BUILD_ALL=false
            BUILD_PYTHON=true
            shift
            ;;
        -v|--verbose)
            VERBOSE=true
            shift
            ;;
        --clean)
            CLEAN=true
            shift
            ;;
        *)
            log_error "Unknown option: $1"
            show_help
            exit 1
            ;;
    esac
done

# If no specific wrapper is selected, build all
if [[ "$BUILD_ALL" == true ]]; then
    BUILD_CSHARP=true
    BUILD_JAVA=true
    BUILD_PYTHON=true
fi

# Get script directory
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$SCRIPT_DIR"

log_info "Building CLI wrappers from: $PROJECT_ROOT"

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to build C# wrapper
build_csharp() {
    log_info "Building C# CLI wrapper..."
    
    local csharp_dir="$PROJECT_ROOT/SdkTestAutomation.CliWrappers/SdkTestAutomation.CSharp"
    
    if [[ ! -d "$csharp_dir" ]]; then
        log_error "C# wrapper directory not found: $csharp_dir"
        return 1
    fi
    
    cd "$csharp_dir"
    
    if [[ "$CLEAN" == true ]]; then
        log_info "Cleaning C# build artifacts..."
        dotnet clean --verbosity quiet
    fi
    
    log_info "Restoring C# dependencies..."
    dotnet restore --verbosity quiet
    
    log_info "Building C# wrapper..."
    if [[ "$VERBOSE" == true ]]; then
        dotnet build --configuration Debug --verbosity normal
    else
        dotnet build --configuration Debug --verbosity quiet
    fi
    
    # Check if build was successful
    local output_dir="$csharp_dir/bin/Debug/net8.0"
    if [[ -f "$output_dir/SdkTestAutomation.CSharp" ]] || [[ -f "$output_dir/SdkTestAutomation.CSharp.exe" ]]; then
        log_success "C# wrapper built successfully"
        log_info "Output location: $output_dir"
    else
        log_error "C# wrapper build failed - executable not found"
        return 1
    fi
}

# Function to build Java wrapper
build_java() {
    log_info "Building Java CLI wrapper..."
    
    local java_dir="$PROJECT_ROOT/SdkTestAutomation.CliWrappers/SdkTestAutomation.Java"
    
    if [[ ! -d "$java_dir" ]]; then
        log_error "Java wrapper directory not found: $java_dir"
        return 1
    fi
    
    cd "$java_dir"
    
    if [[ "$CLEAN" == true ]]; then
        log_info "Cleaning Java build artifacts..."
        mvn clean --quiet
    fi
    
    log_info "Building Java wrapper..."
    if [[ "$VERBOSE" == true ]]; then
        mvn package -DskipTests
    else
        mvn package -DskipTests --quiet
    fi
    
    # Check if build was successful
    local jar_file="$java_dir/target/sdk-wrapper-1.0.0.jar"
    if [[ -f "$jar_file" ]]; then
        log_success "Java wrapper built successfully"
        log_info "Output location: $jar_file"
    else
        log_error "Java wrapper build failed - JAR file not found"
        return 1
    fi
}

# Function to build Python wrapper
build_python() {
    log_info "Building Python CLI wrapper..."
    
    local python_dir="$PROJECT_ROOT/SdkTestAutomation.CliWrappers/SdkTestAutomation.Python"
    
    if [[ ! -d "$python_dir" ]]; then
        log_error "Python wrapper directory not found: $python_dir"
        return 1
    fi
    
    cd "$python_dir"
    
    # Check if virtual environment exists
    local venv_dir="$python_dir/venv"
    if [[ ! -d "$venv_dir" ]]; then
        log_info "Creating Python virtual environment..."
        python3 -m venv venv
    fi
    
    # Activate virtual environment
    log_info "Activating Python virtual environment..."
    source venv/bin/activate
    
    if [[ "$CLEAN" == true ]]; then
        log_info "Cleaning Python build artifacts..."
        pip uninstall -y sdk-wrapper 2>/dev/null || true
        rm -rf build/ dist/ *.egg-info/ 2>/dev/null || true
    fi
    
    log_info "Installing Python dependencies..."
    pip install -e . --quiet
    
    # Check if installation was successful
    if python -c "import sdk_wrapper" 2>/dev/null; then
        log_success "Python wrapper built successfully"
        log_info "Virtual environment: $venv_dir"
        log_info "Python executable: $venv_dir/bin/python"
    else
        log_error "Python wrapper build failed - module not found"
        return 1
    fi
    
    # Deactivate virtual environment
    deactivate
}

# Check prerequisites
check_prerequisites() {
    log_info "Checking build prerequisites..."
    
    local missing_deps=()
    
    # Check for .NET
    if ! command_exists dotnet; then
        missing_deps+=(".NET SDK")
    else
        local dotnet_version=$(dotnet --version)
        log_info "Found .NET SDK: $dotnet_version"
    fi
    
    # Check for Java
    if ! command_exists java; then
        missing_deps+=("Java")
    else
        local java_version=$(java -version 2>&1 | head -n 1)
        log_info "Found Java: $java_version"
    fi
    
    # Check for Maven
    if ! command_exists mvn; then
        missing_deps+=("Maven")
    else
        local mvn_version=$(mvn --version 2>&1 | head -n 1)
        log_info "Found Maven: $mvn_version"
    fi
    
    # Check for Python
    if ! command_exists python3; then
        missing_deps+=("Python 3")
    else
        local python_version=$(python3 --version)
        log_info "Found Python: $python_version"
    fi
    
    if [[ ${#missing_deps[@]} -gt 0 ]]; then
        log_error "Missing dependencies: ${missing_deps[*]}"
        log_error "Please install the missing dependencies and try again"
        exit 1
    fi
    
    log_success "All prerequisites found"
}

# Main build function
main() {
    log_info "Starting CLI wrappers build process..."
    
    # Check prerequisites
    check_prerequisites
    
    local build_errors=0
    
    # Build C# wrapper
    if [[ "$BUILD_CSHARP" == true ]]; then
        if build_csharp; then
            log_success "C# wrapper build completed"
        else
            log_error "C# wrapper build failed"
            ((build_errors++))
        fi
    fi
    
    # Build Java wrapper
    if [[ "$BUILD_JAVA" == true ]]; then
        if build_java; then
            log_success "Java wrapper build completed"
        else
            log_error "Java wrapper build failed"
            ((build_errors++))
        fi
    fi
    
    # Build Python wrapper
    if [[ "$BUILD_PYTHON" == true ]]; then
        if build_python; then
            log_success "Python wrapper build completed"
        else
            log_error "Python wrapper build failed"
            ((build_errors++))
        fi
    fi
    
    # Summary
    echo ""
    if [[ $build_errors -eq 0 ]]; then
        log_success "All requested wrappers built successfully!"
        log_info "You can now run tests using: ./scripts/run-tests.sh"
    else
        log_error "$build_errors wrapper(s) failed to build"
        exit 1
    fi
}

# Run main function
main "$@" 