#!/bin/bash

# Build script for Conductor Go SDK Bridge
# This script builds the shared library for the current platform

set -e

echo "Building Conductor Go SDK Bridge..."

# Create build artifacts directory if it doesn't exist
mkdir -p ../build-artifacts

# Determine platform
case "$(uname -s)" in
    Darwin*)
        echo "Building for macOS..."
        go build -buildmode=c-shared -o ../build-artifacts/conductor-go-bridge.dylib conductor-go-bridge.go
        echo "Built: ../build-artifacts/conductor-go-bridge.dylib"
        ;;
    Linux*)
        echo "Building for Linux..."
        go build -buildmode=c-shared -o ../build-artifacts/conductor-go-bridge.so conductor-go-bridge.go
        echo "Built: ../build-artifacts/conductor-go-bridge.so"
        ;;
    MINGW*|MSYS*|CYGWIN*)
        echo "Building for Windows..."
        go build -buildmode=c-shared -o ../build-artifacts/conductor-go-bridge.dll conductor-go-bridge.go
        echo "Built: ../build-artifacts/conductor-go-bridge.dll"
        ;;
    *)
        echo "Unsupported platform: $(uname -s)"
        exit 1
        ;;
esac

echo "Build completed successfully!"