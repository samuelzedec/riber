#!/bin/bash

# Script to generate JWT certificates for SnackFlow
set -e

echo "Generating JWT certificates..."

# Find project root automatically
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

if [[ "$ASPNETCORE_ENVIRONMENT" == "Development" ]] && [[ -n "$CONTAINER_ENV" ]]; then
    CERT_PATH="/app/Common/Certificates"
    DISPLAY_PATH="/app/Common/Certificates"
    ACCESS_PASSWORD="${AccessTokenSettings__Password:-token@access}"
    REFRESH_PASSWORD="${RefreshTokenSettings__Password:-token@refresh}"
else
    CERT_PATH="$PROJECT_ROOT/src/Riber.Api/Common/Certificates"
    DISPLAY_PATH="src/Riber.Api/Common/Certificates"
    ACCESS_PASSWORD="token@access"
    REFRESH_PASSWORD="token@refresh"
fi

# Create folder if it doesn't exist
mkdir -p "$CERT_PATH"

# Navigate to certificates folder
cd "$CERT_PATH"

rm -f *.pem *.pfx
rm -rf access-token/ refresh-token/ 2>/dev/null || true

openssl genrsa -out access-token-private-key.pem 2048 2>/dev/null
openssl rsa -in access-token-private-key.pem -pubout -out access-token-public-key.pem 2>/dev/null
openssl req -new -x509 -key access-token-private-key.pem -out access-token-certificate.pem -days 365 \
    -subj "/C=BR/ST=AM/L=Manaus/O=SnackFlow/OU=IT/CN=access-token" 2>/dev/null
openssl pkcs12 -export -out access-token-jwt-key.pfx \
    -inkey access-token-private-key.pem \
    -in access-token-certificate.pem \
    -passout pass:$ACCESS_PASSWORD 2>/dev/null

openssl genrsa -out refresh-token-private-key.pem 2048 2>/dev/null
openssl rsa -in refresh-token-private-key.pem -pubout -out refresh-token-public-key.pem 2>/dev/null
openssl req -new -x509 -key refresh-token-private-key.pem -out refresh-token-certificate.pem -days 365 \
    -subj "/C=BR/ST=AM/L=Manaus/O=SnackFlow/OU=IT/CN=refresh-token" 2>/dev/null
openssl pkcs12 -export -out refresh-token-jwt-key.pfx \
    -inkey refresh-token-private-key.pem \
    -in refresh-token-certificate.pem \
    -passout pass:$REFRESH_PASSWORD 2>/dev/null

rm -f *.pem

echo ""
echo "Certificates generated successfully!"
echo "Location: $DISPLAY_PATH"
echo "Access Token PFX Password: $ACCESS_PASSWORD"
echo "Refresh Token PFX Password: $REFRESH_PASSWORD"
echo ""
echo "⚠️ Note: Any existing certificates were replaced with new ones."