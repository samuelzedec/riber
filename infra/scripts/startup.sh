#!/bin/bash
set -e

echo "Starting SnackFlow application..."

# 1. Generate certificates
/usr/local/bin/generate-certificates.sh

# 2. Start application
echo "Starting API server..."
exec dotnet SnackFlow.Api.dll