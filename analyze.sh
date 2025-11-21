#!/bin/bash

set -e

# Validar variáveis obrigatórias
if [[ -z "$SONAR_TOKEN" ]]; then
  echo "Erro: SONAR_TOKEN não definido"
  exit 1
fi

# Configurar variáveis
TOKEN="$SONAR_TOKEN"
PROJECT_KEY="${SONAR_PROJECT_KEY:-samuelzedec_riber}"
ORGANIZATION="${SONAR_ORGANIZATION:-samuelzedec}"
HOST_URL="https://sonarcloud.io"
SOLUTION_FILE="${SOLUTION_FILE:-Riber.slnx}"

echo "Iniciando análise SonarCloud..."
echo "  Projeto: $PROJECT_KEY"
echo "  Organização: $ORGANIZATION"

# Begin
dotnet sonarscanner begin \
  /k:"$PROJECT_KEY" \
  /o:"$ORGANIZATION" \
  /d:sonar.host.url="$HOST_URL" \
  /d:sonar.token="$TOKEN" \
  /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml" \
  /d:sonar.exclusions="\
**/Migrations/**,\
**/bin/**,\
**/obj/**,\
**/appsettings*.json,\
**/Dockerfile*,\
**/Seeders/**,\
**/Schedulers/**,\
**/*.html,\
**/Mappings/**" \
  /d:sonar.coverage.exclusions="\
**/Migrations/**,\
**/Program.cs,\
**/*Validator.cs,\
**/Extensions/**,\
**/Repositories/**,\
**/Settings/**,\
**/Interceptors/**,\
**/Constants/**,\
**/Dtos/**,\
**/DomainEvents/**,\
**/Exceptions/**,\
**/Abstractions/**,\
**/DependencyInjection.cs,\
**/Identity/**,\
**/Common/Api/**,\
**/Requests/**,\
**/UserManagementService.cs,\
**/UserMappingService.cs,\
**/EventHandlers/**,\
**/Services/AI/**"

# Build
echo "Building solution..."
dotnet build "$SOLUTION_FILE" --no-incremental

# Test with coverage
echo "Running tests with coverage..."
dotnet test "$SOLUTION_FILE" \
  --no-build \
  --logger trx \
  /p:CollectCoverage=true \
  /p:CoverletOutputFormat=opencover \
  /p:CoverletOutput=./coverage.opencover.xml

# End
echo "Uploading results to SonarCloud..."
dotnet sonarscanner end /d:sonar.token="$TOKEN"

echo "Análise concluída com sucesso!"