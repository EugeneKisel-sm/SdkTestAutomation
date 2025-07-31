#!/bin/bash

set -e

echo "Building Java CLI applications..."

if ! java -version 2>&1 | grep -q "version \"2[1-9]\|version \"3[0-9]"; then
    echo "Error: Java 21+ is required"
    exit 1
fi

mvn clean package

# Copy JARs to output directories (both Debug and Release for compatibility)
echo "Copying JAR files to output directories..."

# Create directories
mkdir -p ../../../bin/Debug/net8.0/lib
mkdir -p ../../../bin/Release/net8.0/lib

if [ -f "target/conductor-java-cli.jar" ]; then
    cp target/conductor-java-cli.jar ../../../bin/Debug/net8.0/lib/conductor-client.jar
    cp target/conductor-java-cli.jar ../../../bin/Release/net8.0/lib/conductor-client.jar
    echo "✓ Copied conductor-java-cli.jar to conductor-client.jar (Debug & Release)"
else
    echo "Error: conductor-java-cli.jar not found in target directory"
    echo "Available files in target directory:"
    ls -la target/*.jar 2>/dev/null || echo "No JAR files found"
    exit 1
fi

if [ -f "target/orkes-java-cli.jar" ]; then
    cp target/orkes-java-cli.jar ../../../bin/Debug/net8.0/lib/orkes-conductor-client.jar
    cp target/orkes-java-cli.jar ../../../bin/Release/net8.0/lib/orkes-conductor-client.jar
    echo "✓ Copied orkes-java-cli.jar to orkes-conductor-client.jar (Debug & Release)"
else
    echo "Error: orkes-java-cli.jar not found in target directory"
    echo "Available files in target directory:"
    ls -la target/*.jar 2>/dev/null || echo "No JAR files found"
    exit 1
fi

echo "Build completed successfully!" 