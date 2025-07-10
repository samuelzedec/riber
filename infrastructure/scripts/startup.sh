#!/bin/bash
set -e

echo "🚀 Iniciando SnackFlow..."

# 1. Gera certificados
/usr/local/bin/generate-certificates.sh

# 3. Inicia aplicação
echo "▶️ Iniciando API..."
exec dotnet SnackFlow.Api.dll