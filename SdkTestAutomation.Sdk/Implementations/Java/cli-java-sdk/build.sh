#!/bin/bash

set -e

echo "Building Java CLI applications..."

if ! java -version 2>&1 | grep -q "version \"1[7-9]\|version \"2[0-9]"; then
    echo "Error: Java 17+ is required"
    exit 1
fi

mvn clean package

# Copy JARs to output directory
mkdir -p ../../bin/Debug/net8.0/lib
cp target/conductor-java-cli.jar ../../bin/Debug/net8.0/lib/conductor-client.jar
cp target/orkes-java-cli.jar ../../bin/Debug/net8.0/lib/orkes-conductor-client.jar

echo "Build completed successfully!" 