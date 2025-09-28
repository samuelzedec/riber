FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /build

RUN apk add --no-cache bash dos2unix

COPY Directory.Packages.props ./
COPY Directory.Build.props ./
COPY Riber.slnx ./

COPY src/Riber.Api/Riber.Api.csproj ./src/Riber.Api/
COPY src/Riber.Application/Riber.Application.csproj ./src/Riber.Application/
COPY src/Riber.Domain/Riber.Domain.csproj ./src/Riber.Domain/
COPY src/Riber.Infrastructure/Riber.Infrastructure.csproj ./src/Riber.Infrastructure/

COPY tests/Riber.Application.Tests/Riber.Application.Tests.csproj ./tests/Riber.Application.Tests/
COPY tests/Riber.Domain.Tests/Riber.Domain.Tests.csproj ./tests/Riber.Domain.Tests/
COPY tests/Riber.Infrastructure.Tests/Riber.Infrastructure.Tests.csproj ./tests/Riber.Infrastructure.Tests/

RUN dotnet restore Riber.slnx
COPY . .

RUN dotnet build Riber.slnx -c Release --no-restore
RUN dotnet test Riber.slnx -c Release --no-build --no-restore

RUN dotnet publish src/Riber.Api/Riber.Api.csproj -c Release -o /app/publish --no-build --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

ARG CONNECTION_STRING
ARG ACCESS_TOKEN_SECRET_KEY
ARG REFRESH_TOKEN_SECRET_KEY

RUN test -n "$CONNECTION_STRING" || (echo "ERROR: CONNECTION_STRING is required" && exit 1)
RUN test -n "$ACCESS_TOKEN_SECRET_KEY" || (echo "ERROR: ACCESS_TOKEN_SECRET_KEY is required" && exit 1) 
RUN test -n "$REFRESH_TOKEN_SECRET_KEY" || (echo "ERROR: REFRESH_TOKEN_SECRET_KEY is required" && exit 1)

RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -s /bin/sh -D appuser

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection=$CONNECTION_STRING
ENV AccessTokenSettings__SecretKey=$ACCESS_TOKEN_SECRET_KEY
ENV RefreshTokenSettings__SecretKey=$REFRESH_TOKEN_SECRET_KEY
ENV S3__BucketImagesName=riber-bucket-images

RUN apk add --no-cache bash
COPY --from=build /app/publish .

RUN chown -R appuser:appgroup /app
USER appuser

ENTRYPOINT ["dotnet", "Riber.Api.dll"]