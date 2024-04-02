#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine-composite AS base
WORKDIR /app
RUN apk update && apk add curl
RUN rm -rf /var/cache/apk/*
HEALTHCHECK --interval=10s --timeout=5s --retries=5 --start-period=30s \
  CMD curl -f http://localhost:8080 || exit 1
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
COPY ["Client/eShopping.Client/eShopping.Client.csproj", "./Client/eShopping.Client/"]
COPY ["Services/Basket/Basket.Core/Basket.Core.csproj", "./Services/Basket/Basket.Core/"]
COPY ["Services/Catalog/Catalog.Core/Catalog.Core.csproj", "./Services/Catalog/Catalog.Core/"]
COPY ["Services/Shared/Shared.Core/Shared.Core.csproj", "./Services/Shared/Shared.Core/"]
COPY ["Services/Users/Users.Core/Users.Core.csproj", "./Services/Users/Users.Core/"]
WORKDIR "/src/Client/eShopping.Client"
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \
#    dotnet restore "./eShopping.Client.csproj" -r $BUILD_PLATFORM --disable-parallel
    dotnet restore "./eShopping.Client.csproj" \
                   -r $BUILD_PLATFORM

FROM restorer AS build
WORKDIR /src
#COPY . .
COPY ./Client/ ./Client/
COPY ./Services/Basket/Basket.Core/ ./Services/Basket/Basket.Core/
COPY ./Services/Catalog/Catalog.Core/ ./Services/Catalog/Catalog.Core/
COPY ./Services/Shared/Shared.Core/ ./Services/Shared/Shared.Core/
COPY ./Services/Users/Users.Core/ ./Services/Users/Users.Core/
WORKDIR "/src/Client/eShopping.Client"
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \
    dotnet build "./eShopping.Client.csproj" \
                 -c $BUILD_CONFIGURATION \
                 -r $BUILD_PLATFORM \
                 --self-contained false \
                 --no-restore \
                 /p:Optimize=true \
                 /p:LinkDuringPublish=true \
                 -o /app/build

FROM build AS publish
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \
    dotnet publish "./eShopping.Client.csproj" \
                   -c $BUILD_CONFIGURATION \
                   -r $BUILD_PLATFORM \
                   --self-contained false \
                   --no-restore \
                   /p:Optimize=true \
                   /p:LinkDuringPublish=true \
                   /p:UseAppHost=true \
                   -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "/app/eShopping.Client.dll"]