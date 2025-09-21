#!/bin/bash

# Script to generate JWT certificates for SnackFlow
set -e

echo "Generating JWT certificates..."

# Find project root automatically
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

# Detect environment and set path/password
if [[ "$ASPNETCORE_ENVIRONMENT" == "Development" ]] && [[ -n "$CONTAINER_ENV" ]]; then
    # Docker container
    CERT_PATH="/app/Common/Certificates"
    PASSWORD="${CERT_PASSWORD:-root-container}"
    echo "Environment: Docker Development"
else
    # Local development - calculate path automatically
    CERT_PATH="$PROJECT_ROOT/src/Riber.Api/Common/Certificates"
    PASSWORD="root@dev"
    echo "Environment: Local Development"
fi

echo "Generating certificates in: $CERT_PATH with password: $PASSWORD"

# Create folder if it doesn't exist
mkdir -p "$CERT_PATH"

# Navigate to certificates folder
cd "$CERT_PATH"

# Clean old certificates
rm -f *.pem *.pfx
rm -rf access-token/ refresh-token/ 2>/dev/null || true

echo "Generating Access Token certificate..."
openssl genrsa -out access-token-private-key.pem 2048 2>/dev/null
openssl rsa -in access-token-private-key.pem -pubout -out access-token-public-key.pem 2>/dev/null
openssl req -new -x509 -key access-token-private-key.pem -out access-token-certificate.pem -days 365 \
    -subj "/C=BR/ST=AM/L=Manaus/O=SnackFlow/OU=IT/CN=access-token" 2>/dev/null
openssl pkcs12 -export -out access-token-jwt-key.pfx \
    -inkey access-token-private-key.pem \
    -in access-token-certificate.pem \
    -passout pass:$PASSWORD 2>/dev/null

echo "Generating Refresh Token certificate..."
openssl genrsa -out refresh-token-private-key.pem 2048 2>/dev/null
openssl rsa -in refresh-token-private-key.pem -pubout -out refresh-token-public-key.pem 2>/dev/null
openssl req -new -x509 -key refresh-token-private-key.pem -out refresh-token-certificate.pem -days 365 \
    -subj "/C=BR/ST=AM/L=Manaus/O=SnackFlow/OU=IT/CN=refresh-token" 2>/dev/null
openssl pkcs12 -export -out refresh-token-jwt-key.pfx \
    -inkey refresh-token-private-key.pem \
    -in refresh-token-certificate.pem \
    -passout pass:$PASSWORD 2>/dev/null

echo "Cleaning temporary files..."
# Remove all .pem files, keep only .pfx
rm -f *.pem

echo "Certificates generated successfully in: $CERT_PATH"
echo "PFX Password: $PASSWORD"