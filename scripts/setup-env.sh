#!/bin/bash

# SdkTestAutomation Environment Setup Script
# 
# This script helps you set up the environment for SdkTestAutomation.
# It creates a .env file and validates the setup.

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_header() {
    echo -e "${BLUE}=== SdkTestAutomation Environment Setup ===${NC}"
    echo ""
}

print_help() {
    cat << EOF
SdkTestAutomation Environment Setup

This script helps you set up the environment for SdkTestAutomation.

Usage:
  $0 [options]

Options:
  --minimal     Use minimal environment configuration
  --full        Use full environment configuration (default)
  --help        Show this help message
  --validate    Validate existing .env file

Examples:
  $0 --minimal    # Quick setup with essential variables only
  $0 --full       # Full setup with all configuration options
  $0 --validate   # Validate existing .env file
EOF
}

setup_minimal_env() {
    echo -e "${BLUE}Setting up minimal environment configuration...${NC}"
    
    if [ -f "SdkTestAutomation.Tests/.env" ]; then
        echo -e "${YELLOW}⚠️  .env file already exists. Backing up to .env.backup${NC}"
        cp SdkTestAutomation.Tests/.env SdkTestAutomation.Tests/.env.backup
    fi
    
    cp SdkTestAutomation.Tests/env.example SdkTestAutomation.Tests/.env
    echo -e "${GREEN}✅ Minimal .env file created in SdkTestAutomation.Tests/${NC}"
    echo -e "${YELLOW}📝 Please edit SdkTestAutomation.Tests/.env file with your Conductor server URL${NC}"
}

setup_full_env() {
    echo -e "${BLUE}Setting up full environment configuration...${NC}"
    
    if [ -f "SdkTestAutomation.Tests/.env" ]; then
        echo -e "${YELLOW}⚠️  .env file already exists. Backing up to .env.backup${NC}"
        cp SdkTestAutomation.Tests/.env SdkTestAutomation.Tests/.env.backup
    fi
    
    cp SdkTestAutomation.Tests/env.template SdkTestAutomation.Tests/.env
    echo -e "${GREEN}✅ Full .env file created in SdkTestAutomation.Tests/${NC}"
    echo -e "${YELLOW}📝 Please edit SdkTestAutomation.Tests/.env file with your configuration${NC}"
}

validate_env() {
    echo -e "${BLUE}Validating .env file...${NC}"
    
    if [ ! -f "SdkTestAutomation.Tests/.env" ]; then
        echo -e "${RED}❌ .env file not found in SdkTestAutomation.Tests/${NC}"
        echo -e "${YELLOW}   Run this script without --validate to create one${NC}"
        return 1
    fi
    
    # Check for required variables
    local missing_vars=()
    
    if ! grep -q "^CONDUCTOR_SERVER_URL=" SdkTestAutomation.Tests/.env; then
        missing_vars+=("CONDUCTOR_SERVER_URL")
    fi
    
    if [ ${#missing_vars[@]} -gt 0 ]; then
        echo -e "${RED}❌ Missing required variables: ${missing_vars[*]}${NC}"
        return 1
    fi
    
    # Check for common issues
    local issues=()
    
    if grep -q "^CONDUCTOR_SERVER_URL=$" SdkTestAutomation.Tests/.env; then
        issues+=("CONDUCTOR_SERVER_URL is empty")
    fi
    
    if [ ${#issues[@]} -gt 0 ]; then
        echo -e "${YELLOW}⚠️  Issues found:${NC}"
        for issue in "${issues[@]}"; do
            echo -e "${YELLOW}   - $issue${NC}"
        done
        return 1
    fi
    
    echo -e "${GREEN}✅ .env file is valid${NC}"
    
    # Show current configuration
    echo -e "${BLUE}Current configuration:${NC}"
    grep -E "^(CONDUCTOR_SERVER_URL|SDK_TYPE|CONDUCTOR_AUTH_KEY|CONDUCTOR_AUTH_SECRET)=" SdkTestAutomation.Tests/.env | while read line; do
        echo -e "${GREEN}   $line${NC}"
    done
}

check_prerequisites() {
    echo -e "${BLUE}Checking prerequisites...${NC}"
    
    local missing=()
    
    # Check .NET
    if ! command -v dotnet &> /dev/null; then
        missing+=(".NET SDK")
    else
        echo -e "${GREEN}✅ .NET SDK found${NC}"
    fi
    
    # Check Java (optional)
    if command -v java &> /dev/null && command -v mvn &> /dev/null; then
        echo -e "${GREEN}✅ Java and Maven found${NC}"
    else
        echo -e "${YELLOW}⚠️  Java or Maven not found - Java SDK tests will be skipped${NC}"
    fi
    
    # Check Python (optional)
    if command -v python3 &> /dev/null; then
        echo -e "${GREEN}✅ Python found${NC}"
    else
        echo -e "${YELLOW}⚠️  Python3 not found - Python SDK tests will be skipped${NC}"
    fi
    
    if [ ${#missing[@]} -gt 0 ]; then
        echo -e "${RED}❌ Missing prerequisites: ${missing[*]}${NC}"
        echo -e "${YELLOW}   Please install the missing prerequisites and run this script again${NC}"
        return 1
    fi
    
    echo -e "${GREEN}✅ Prerequisites check completed${NC}"
}

main() {
    print_header
    
    case "${1:-}" in
        --help|-h)
            print_help
            exit 0
            ;;
        --validate)
            validate_env
            exit $?
            ;;
        --minimal)
            setup_minimal_env
            ;;
        --full|"")
            setup_full_env
            ;;
        *)
            echo -e "${RED}❌ Invalid option: $1${NC}"
            print_help
            exit 1
            ;;
    esac
    
    echo ""
    echo -e "${BLUE}Next steps:${NC}"
    echo -e "${GREEN}1. Edit SdkTestAutomation.Tests/.env file with your configuration${NC}"
    echo -e "${GREEN}2. Start Conductor server: docker run -d -p 8080:8080 conductoross/conductor-server:latest${NC}"
    echo -e "${GREEN}3. Run tests: ./scripts/run-tests.sh --validate${NC}"
    echo ""
    echo -e "${BLUE}For more information, see:${NC}"
    echo -e "${GREEN}   - README.md${NC}"
    echo -e "${GREEN}   - SDK_INTEGRATION_GUIDE.md${NC}"
    echo -e "${GREEN}   - SdkTestAutomation.Tests/env.template (for all available options)${NC}"
}

# Run main function with all arguments
main "$@" 