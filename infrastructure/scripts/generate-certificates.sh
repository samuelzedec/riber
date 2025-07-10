#!/bin/bash

# Script para gerar certificados JWT para SnackFlow
set -e

echo "🔐 Gerando certificados JWT..."

# Encontra a raiz do projeto automaticamente
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"

# Detecta ambiente e define caminho/senha
if [[ "$ASPNETCORE_ENVIRONMENT" == "Development" ]] && [[ -n "$CONTAINER_ENV" ]]; then
    # Docker container
    CERT_PATH="/app/Common/Certificates"
    PASSWORD="${CERT_PASSWORD:-root-container}"
    echo "🐳 Ambiente: Docker Development"
else
    # Local development - calcula caminho automaticamente
    CERT_PATH="$PROJECT_ROOT/src/SnackFlow.Api/Common/Certificates"
    PASSWORD="root"
    echo "💻 Ambiente: Local Development"
fi

echo "🔐 Gerando certificados em: $CERT_PATH com senha: $PASSWORD"

# Cria pasta se não existir
mkdir -p "$CERT_PATH"

# Navega para a pasta de certificados
cd "$CERT_PATH"

# Limpa certificados antigos
rm -f *.pem *.pfx
rm -rf access-token/ refresh-token/ 2>/dev/null || true

echo "🔑 Gerando Access Token..."
openssl genrsa -out access-token-private-key.pem 2048 2>/dev/null
openssl rsa -in access-token-private-key.pem -pubout -out access-token-public-key.pem 2>/dev/null
openssl req -new -x509 -key access-token-private-key.pem -out access-token-certificate.pem -days 365 \
    -subj "/C=BR/ST=AM/L=Manaus/O=SnackFlow/OU=IT/CN=access-token" 2>/dev/null
openssl pkcs12 -export -out access-token-jwt-key.pfx \
    -inkey access-token-private-key.pem \
    -in access-token-certificate.pem \
    -passout pass:$PASSWORD 2>/dev/null

echo "🔄 Gerando Refresh Token..."
openssl genrsa -out refresh-token-private-key.pem 2048 2>/dev/null
openssl rsa -in refresh-token-private-key.pem -pubout -out refresh-token-public-key.pem 2>/dev/null
openssl req -new -x509 -key refresh-token-private-key.pem -out refresh-token-certificate.pem -days 365 \
    -subj "/C=BR/ST=AM/L=Manaus/O=SnackFlow/OU=IT/CN=refresh-token" 2>/dev/null
openssl pkcs12 -export -out refresh-token-jwt-key.pfx \
    -inkey refresh-token-private-key.pem \
    -in refresh-token-certificate.pem \
    -passout pass:$PASSWORD 2>/dev/null

echo "🧹 Limpando arquivos temporários..."
# Remove todos os .pem, mantém apenas os .pfx
rm -f *.pem

echo "✅ Certificados gerados em: $CERT_PATH"
echo "✅ Senha PFX: $PASSWORD"