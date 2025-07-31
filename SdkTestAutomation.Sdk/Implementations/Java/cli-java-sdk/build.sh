#!/bin/bash

# Build script for Java CLI applications

set -e

echo "Building Java CLI applications..."

# Check if Maven is installed
if ! command -v mvn &> /dev/null; then
    echo "Error: Maven is not installed. Please install Maven first."
    exit 1
fi

# Check if Java 17+ is installed
if ! command -v java &> /dev/null; then
    echo "Error: Java is not installed. Please install Java 17+ first."
    exit 1
fi

# Get Java version
JAVA_VERSION=$(java -version 2>&1 | head -n 1 | cut -d'"' -f2 | cut -d'.' -f1)
if [ "$JAVA_VERSION" -lt 17 ]; then
    echo "Error: Java 17+ is required. Current version: $JAVA_VERSION"
    exit 1
fi

echo "Using Java version: $(java -version 2>&1 | head -n 1)"

# Build the applications
cd "$(dirname "$0")"

echo "Building Java CLI applications..."
mvn clean package -DskipTests

# Copy the JAR files to the output directory
mkdir -p ../../bin/Debug/net8.0/lib
cp target/conductor-java-cli.jar ../../bin/Debug/net8.0/lib/conductor-client.jar
cp target/orkes-java-cli.jar ../../bin/Debug/net8.0/lib/orkes-conductor-client.jar

echo "Java CLI applications built successfully!"
echo "JAR files copied to:"
echo "  - ../../bin/Debug/net8.0/lib/conductor-client.jar"
echo "  - ../../bin/Debug/net8.0/lib/orkes-conductor-client.jar" 