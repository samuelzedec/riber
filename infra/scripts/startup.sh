#!/bin/bash
set -e

echo "Starting Riber application..."

# 1. Generate certificates
/usr/local/bin/generate-certificates.sh

# 2. Start application
echo "Starting API server..."
exec dotnet Riber.Api.dll