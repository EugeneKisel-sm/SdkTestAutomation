#!/bin/bash

# Build script for Go shared library
# This compiles the Go code into a DLL that can be used directly by .NET

set -e

echo "Building Go shared library..."

# Check if Go is installed
if ! command -v go &> /dev/null; then
    echo "ERROR: Go is not installed. Please install Go 1.19+ first."
    exit 1
fi

# Check Go version
GO_VERSION=$(go version | cut -d' ' -f3 | sed 's/go//')
echo "Go version: $GO_VERSION"

# Set environment variables for CGO
export CGO_ENABLED=1
export GOOS=windows  # Change to linux or darwin for other platforms
export GOARCH=amd64

# Build the shared library
echo "Compiling conductor-go-bridge.dll..."
go build -buildmode=c-shared -o conductor-go-bridge.dll SdkTestAutomation.Sdk/Implementations/Go/conductor-go-bridge.go

if [ $? -eq 0 ]; then
    echo "✅ Successfully built conductor-go-bridge.dll"
    echo "📁 Library location: $(pwd)/conductor-go-bridge.dll"
    
    # Copy to the appropriate location
    cp conductor-go-bridge.dll SdkTestAutomation.Sdk/Implementations/Go/
    echo "📋 Copied to SdkTestAutomation.Sdk/Implementations/Go/"
    
    # Show file info
    ls -la conductor-go-bridge.dll
else
    echo "❌ Failed to build Go shared library"
    exit 1
fi

echo ""
echo "🎉 Go shared library build complete!"
echo ""
echo "Next steps:"
echo "1. Update your Go adapters to use GoSharedLibraryClient instead of GoHttpClient"
echo "2. Make sure the DLL is in the same directory as your .NET executable"
echo "3. Test the integration" 