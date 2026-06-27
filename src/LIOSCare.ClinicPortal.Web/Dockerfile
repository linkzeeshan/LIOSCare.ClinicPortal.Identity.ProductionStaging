# Use the official .NET 10.0 runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Install runtime dependencies used by health checks and Npgsql/GSSAPI
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl libkrb5-3 \
    && rm -rf /var/lib/apt/lists/*

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["LIOSCare.ClinicPortal.Web.csproj", "./"]
RUN dotnet restore "./LIOSCare.ClinicPortal.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/."
RUN dotnet build "LIOSCare.ClinicPortal.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LIOSCare.ClinicPortal.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create logs/uploads directories used by mounted volumes
RUN mkdir -p /app/logs /app/wwwroot/uploads

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Health check must match ASPNETCORE_URLS/container port
HEALTHCHECK --interval=30s --timeout=10s --start-period=20s --retries=3 \
  CMD curl -fsS http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "LIOSCare.ClinicPortal.Web.dll"]
