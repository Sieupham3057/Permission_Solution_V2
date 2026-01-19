# =========================================================
# Customer.API - Production-ready Dockerfile (.NET 9)
# Location: Services/Customer/Customer.API/Dockerfile
# Build context: solution root (folder contains .sln)
# =========================================================

# =========================
# 1) RESTORE STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore
WORKDIR /src

# Copy csproj files first for better cache
COPY Services/Customer/Customer.API/Customer.API.csproj Services/Customer/Customer.API/
COPY Services/Customer/Customer.Application/Customer.Application.csproj Services/Customer/Customer.Application/
COPY Services/Customer/Customer.Infrastructure/Customer.Infrastructure.csproj Services/Customer/Customer.Infrastructure/
COPY Services/Customer/Customer.Domain/Customer.Domain.csproj Services/Customer/Customer.Domain/

# Shared projects
COPY BuildingBlocks/Authorization/Authorization.csproj BuildingBlocks/Authorization/
COPY Contracts/Contracts/Contracts.csproj Contracts/Contracts/

# Restore
RUN dotnet restore Services/Customer/Customer.API/Customer.API.csproj


# =========================
# 2) BUILD / PUBLISH STAGE (OPTIMIZED CACHE)
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only necessary source folders (NOT everything)
COPY Services/Customer/Customer.API/ Services/Customer/Customer.API/
COPY Services/Customer/Customer.Application/ Services/Customer/Customer.Application/
COPY Services/Customer/Customer.Infrastructure/ Services/Customer/Customer.Infrastructure/
COPY Services/Customer/Customer.Domain/ Services/Customer/Customer.Domain/
COPY BuildingBlocks/Authorization/ BuildingBlocks/Authorization/
COPY Contracts/Contracts/ Contracts/Contracts/

RUN dotnet publish Services/Customer/Customer.API/Customer.API.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# 3) RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Runtime env
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0

# Create non-root user
RUN addgroup --system appgroup \
    && adduser --system --ingroup appgroup appuser

# Copy published output
COPY --from=build /app/publish .

# Expose container port
EXPOSE 8080

# Healthcheck (requires your API exposes GET /health)
HEALTHCHECK --interval=30s --timeout=3s --start-period=20s --retries=3 \
  CMD wget -qO- http://localhost:8080/health || exit 1

# Run as non-root
USER appuser

ENTRYPOINT ["dotnet", "Customer.API.dll"]
