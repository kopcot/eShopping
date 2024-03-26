#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.2-alpine3.19-composite-arm64v8 AS base
WORKDIR /app
RUN apk update && apk add curl
RUN rm -rf /var/cache/apk/*
HEALTHCHECK --interval=5s --timeout=10s --retries=5 --start-period=30s \
  CMD curl -f http://localhost:8080/api/health || exit 1
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS restorer
ARG BUILD_CONFIGURATION=Release
ARG BUILD_PLATFORM=linux-arm64
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=false
ENV NUGET_XMLDOC_MODE=skip
ENV NUGET_CERT_REVOCATION_MODE=offline
RUN mkdir /src
WORKDIR /src
COPY ["Services/Catalog/Catalog.API/Catalog.API.csproj", "./Services/Catalog/Catalog.API/"]
COPY ["Services/Catalog/Catalog.Application/Catalog.Application.csproj", "./Services/Catalog/Catalog.Application/"]
COPY ["Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "./Services/Catalog/Catalog.Infrastructure/"]
COPY ["Services/Catalog/Catalog.Core/Catalog.Core.csproj", "./Services/Catalog/Catalog.Core/"]
COPY ["Services/Shared/Shared.Api/Shared.Api.csproj", "./Services/Shared/Shared.Api/"]
COPY ["Services/Shared/Shared.Application/Shared.Application.csproj", "./Services/Shared/Shared.Application/"]
COPY ["Services/Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "./Services/Shared/Shared.Infrastructure/"]
COPY ["Services/Shared/Shared.Core/Shared.Core.csproj", "./Services/Shared/Shared.Core/"]
WORKDIR "/src/Services/Catalog/Catalog.API"
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \
#    dotnet restore "./Catalog.API.csproj" -r $BUILD_PLATFORM --disable-parallel
    dotnet restore "./Catalog.API.csproj" -r $BUILD_PLATFORM

FROM restorer AS build
WORKDIR /src
COPY . .
WORKDIR "/src/Services/Catalog/Catalog.API"
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \
    dotnet build "./Catalog.API.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/build 

FROM build AS publish
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \
    dotnet publish "./Catalog.API.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "/app/Catalog.API.dll"]