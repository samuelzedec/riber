FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /build

RUN apk add --no-cache bash dos2unix

COPY Directory.Packages.props Directory.Build.props ./

COPY src/Riber.Api/Riber.Api.csproj ./src/Riber.Api/
COPY src/Riber.Application/Riber.Application.csproj ./src/Riber.Application/
COPY src/Riber.Domain/Riber.Domain.csproj ./src/Riber.Domain/
COPY src/Riber.Infrastructure/Riber.Infrastructure.csproj ./src/Riber.Infrastructure/

RUN dotnet restore ./src/Riber.Api/Riber.Api.csproj

COPY src/Riber.Api ./src/Riber.Api/
COPY src/Riber.Application ./src/Riber.Application/
COPY src/Riber.Domain ./src/Riber.Domain/
COPY src/Riber.Infrastructure ./src/Riber.Infrastructure/

RUN dotnet publish src/Riber.Api/Riber.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -s /bin/sh -D appuser && \
    apk add --no-cache bash

ENV ConnectionStrings__DefaultConnection="" \
    AccessTokenSettings__SecretKey="" \
    RefreshTokenSettings__SecretKey="" \
    AWS__S3__BucketImagesName="" \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://0.0.0.0:8080

COPY --from=build /app/publish .

RUN chown -R appuser:appgroup /app
USER appuser

EXPOSE 8080
ENTRYPOINT ["dotnet", "Riber.Api.dll"]