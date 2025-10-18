#!/bin/bash

set -e

# Detectar ambiente e configurar variáveis
if [[ -n "$CI" || -n "$GITHUB_ACTIONS" ]]; then
  if [ -z "$SONAR_TOKEN" ]; then
    echo "Erro: SONAR_TOKEN não definido"
    exit 1
  fi
  
  TOKEN="$SONAR_TOKEN"
  PROJECT_KEY="${SONAR_PROJECT_KEY:-samuelzedec_riber}"
  ORGANIZATION="${SONAR_ORGANIZATION:-samuelzedec}"
  HOST_URL="https://sonarcloud.io"
  SOLUTION_FILE="riber.sln"
  USE_ORG_FLAG=true
else
  # Local - SonarQube
  if [ -z "$1" ]; then
    echo "Erro: Token não fornecido"
    echo "Uso: ./analyze.sh SEU_TOKEN"
    exit 1
  fi
  
  TOKEN="$1"
  PROJECT_KEY="riber"
  ORGANIZATION="samuelzedec"
  HOST_URL="http://localhost:9000"
  SOLUTION_FILE="riber.slnx"
  USE_ORG_FLAG=false
fi

# Construir comando base do sonarscanner begin
SONAR_BEGIN_ARGS=(
  /k:"$PROJECT_KEY"
  /d:sonar.host.url="$HOST_URL"
  /d:sonar.token="$TOKEN"
  /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
  /d:sonar.exclusions="\
**/Migrations/**,\
**/bin/**,\
**/obj/**,\
**/appsettings*.json,\
**/Dockerfile*,\
**/Seeders/**,\
**/*Map.cs,\
**/Schedulers/**,\
**/Dispatchers,"
  /d:sonar.coverage.exclusions="\
**/Migrations/**,\
**/Program.cs,\
**/*Validator.cs,\
**/Repositories/**,\
**/Settings/**,\
**/Interceptors/**,\
**/Constants/**,\
**/Models/**,\
**/Exceptions/**,\
**/Abstractions/**,\
**/DependencyInjection.cs,\
**/Identity/**,\
**/Common/Api/**,\
**/Requests/**,\
**/AuthService.cs"
)

# Adicionar /o: apenas se for SonarCloud
if [ "$USE_ORG_FLAG" = true ]; then
  SONAR_BEGIN_ARGS=(/o:"$ORGANIZATION" "${SONAR_BEGIN_ARGS[@]}")
fi

# Begin
dotnet sonarscanner begin "${SONAR_BEGIN_ARGS[@]}"

# Build
dotnet build "$SOLUTION_FILE" --no-incremental

# Test
dotnet test "$SOLUTION_FILE" \
  --no-build \
  --logger trx \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput=./coverage.opencover.xml

# End
dotnet sonarscanner end /d:sonar.token="$TOKEN"
echo "Análise concluída com sucesso"