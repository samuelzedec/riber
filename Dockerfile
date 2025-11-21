FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /build

RUN apk add --no-cache bash dos2unix
COPY Directory.Packages.props Directory.Build.props Riber.slnx ./

COPY src/Riber.Api/Riber.Api.csproj ./src/Riber.Api/
COPY src/Riber.Application/Riber.Application.csproj ./src/Riber.Application/
COPY src/Riber.Domain/Riber.Domain.csproj ./src/Riber.Domain/
COPY src/Riber.Infrastructure/Riber.Infrastructure.csproj ./src/Riber.Infrastructure/

COPY tests/Riber.Api.Tests/Riber.Api.Tests.csproj ./tests/Riber.Api.Tests/
COPY tests/Riber.Application.Tests.Tests/Riber.Application.Tests.csproj ./tests/Riber.Application.Tests/
COPY tests/Riber.Domain.Tests/Riber.Domain.Tests.csproj ./tests/Riber.Domain.Tests/
COPY tests/Riber.Infrastructure.Tests/Riber.Infrastructure.Tests.csproj ./tests/Riber.Infrastructure.Tests/

RUN dotnet restore Riber.slnx
COPY src/ ./src/
COPY tests/ ./tests/

RUN dotnet publish Riber.slnx -c Release --no-restore

FROM build AS tests
WORKDIR /build

RUN dotnet test Riber.slnx \
    -c Release \
    --no-build \
    --no-restore \
    --logger "console;verbosity=detailed"

FROM build AS publish
WORKDIR /build

RUN dotnet publish src/Riber.Api/Riber.Api.csproj \
    -c Release \
    -o /publish \
    --no-restore \
    --no-build

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS runtime
WORKDIR /app

RUN addgroup -g 1000 app-group && \
    adduser -u 1000 -G app-group -s /bin/sh -D app-user && \
    apk add --no-cache bash

ENV ConnectionStrings__DefaultConnection="" \
    AccessTokenSettings__SecretKey="" \
    RefreshTokenSettings__SecretKey="" \
    AWS__S3__BucketImagesName="" \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://0.0.0.0:8080

COPY --from=publish --chown=app-user:app-group /publish .
USER app-user

EXPOSE 8080
ENTRYPOINT ["dotnet", "Riber.Api.dll"]