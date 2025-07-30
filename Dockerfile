# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file
COPY OptimalyBlueprint/OptimalyBlueprint.csproj OptimalyBlueprint/

# Restore dependencies
RUN dotnet restore OptimalyBlueprint/OptimalyBlueprint.csproj

# Copy everything else and build
COPY . .
WORKDIR /src/OptimalyBlueprint
RUN dotnet build OptimalyBlueprint.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish OptimalyBlueprint.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "OptimalyBlueprint.dll"]