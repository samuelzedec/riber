#!/bin/bash

# Script para gerar certificados JWT para SnackFlow
set -e

echo "🔐 Gerando certificados JWT..."

# Verifica se está na pasta correta
if [[ ! -d "src/SnackFlow.Api/Common/Certificates" ]]; then
    echo "❌ Execute da pasta raiz do projeto"
    exit 1
fi

# Navega para a pasta de certificados
cd src/SnackFlow.Api/Common/Certificates/

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
    -passout pass:root 2>/dev/null

echo "🔄 Gerando Refresh Token..."
openssl genrsa -out refresh-token-private-key.pem 2048 2>/dev/null
openssl rsa -in refresh-token-private-key.pem -pubout -out refresh-token-public-key.pem 2>/dev/null
openssl req -new -x509 -key refresh-token-private-key.pem -out refresh-token-certificate.pem -days 365 \
    -subj "/C=BR/ST=AM/L=Manaus/O=SnackFlow/OU=IT/CN=refresh-token" 2>/dev/null
openssl pkcs12 -export -out refresh-token-jwt-key.pfx \
    -inkey refresh-token-private-key.pem \
    -in refresh-token-certificate.pem \
    -passout pass:root 2>/dev/null

echo "🧹 Limpando arquivos temporários..."
# Remove todos os .pem, mantém apenas os .pfx
rm -f *.pem

# Volta para a pasta raiz
cd - > /dev/null

echo "✅ Certificados gerados! Senha PFX: root"