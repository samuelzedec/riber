FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /build

RUN apk add --no-cache bash dos2unix

COPY Directory.Packages.props ./
COPY Directory.Build.props ./

COPY src/Riber.Api/Riber.Api.csproj ./src/Riber.Api/
COPY src/Riber.Application/Riber.Application.csproj ./src/Riber.Application/
COPY src/Riber.Domain/Riber.Domain.csproj ./src/Riber.Domain/
COPY src/Riber.Infrastructure/Riber.Infrastructure.csproj ./src/Riber.Infrastructure/

RUN dotnet restore src/Riber.Api/Riber.Api.csproj
COPY . .

RUN dotnet build src/Riber.Api/Riber.Api.csproj -c Release --no-restore
RUN dotnet publish src/Riber.Api/Riber.Api.csproj -c Release -o /app/publish --no-build --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app

RUN addgroup -g 1000 appgroup && \
    adduser -u 1000 -G appgroup -s /bin/sh -D appuser

ENV ConnectionStrings__DefaultConnection=""
ENV AccessTokenSettings__SecretKey=""
ENV RefreshTokenSettings__SecretKey=""
ENV S3__BucketImagesName=""
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

RUN apk add --no-cache bash
COPY --from=build /app/publish .

RUN chown -R appuser:appgroup /app
USER appuser

EXPOSE 8080
ENTRYPOINT ["dotnet", "Riber.Api.dll"]