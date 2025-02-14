FROM mcr.microsoft.com/dotnet/aspnet:8.0.2-alpine3.19-composite AS base
WORKDIR /app
RUN apk update && apk add curl
RUN rm -rf /var/cache/apk/*
HEALTHCHECK --interval=5s --timeout=10s --retries=5 --start-period=30s \
  CMD curl -f http://localhost:8080/api/health || exit 1
EXPOSE 8080

# RESTORE
FROM mcr.microsoft.com/dotnet/sdk:8.0.201-alpine3.19 AS restorer
ARG BUILD_CONFIGURATION=Release
ARG BUILD_PLATFORM=linux-amd64
ENV DOTNET_NUGET_SIGNATURE_VERIFICATION=false
ENV DOTNET_USE_POLLING_FILE_WATCHER=false
ENV NUGET_XMLDOC_MODE=skip
RUN mkdir /src
WORKDIR /src
COPY ["Client/eShopping.Client/eShopping.Client.csproj", "./Client/eShopping.Client/"]
COPY ["Services/Basket/Basket.Core/Basket.Core.csproj", "./Services/Basket/Basket.Core/"]
COPY ["Services/Basket/Basket.API/Basket.API.csproj", "./Services/Basket/Basket.API/"]
COPY ["Services/Basket/Basket.Application/Basket.Application.csproj", "./Services/Basket/Basket.Application/"]
COPY ["Services/Basket/Basket.Infrastructure/Basket.Infrastructure.csproj", "./Services/Basket/Basket.Infrastructure/"]
COPY ["Services/Catalog/Catalog.Core/Catalog.Core.csproj", "./Services/Catalog/Catalog.Core/"]
COPY ["Services/Catalog/Catalog.API/Catalog.API.csproj", "./Services/Catalog/Catalog.API/"]
COPY ["Services/Catalog/Catalog.Application/Catalog.Application.csproj", "./Services/Catalog/Catalog.Application/"]
COPY ["Services/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "./Services/Catalog/Catalog.Infrastructure/"]
COPY ["Services/Shared/Shared.Core/Shared.Core.csproj", "./Services/Shared/Shared.Core/"]
COPY ["Services/Shared/Shared.Api/Shared.Api.csproj", "./Services/Shared/Shared.Api/"]
COPY ["Services/Shared/Shared.Application/Shared.Application.csproj", "./Services/Shared/Shared.Application/"]
COPY ["Services/Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "./Services/Shared/Shared.Infrastructure/"]
COPY ["Services/Users/Users.Core/Users.Core.csproj", "./Services/Users/Users.Core/"]
COPY ["Services/Users/Users.Api/Users.Api.csproj", "./Services/Users/Users.Api/"]
COPY ["Services/Users/Users.Application/Users.Application.csproj", "./Services/Users/Users.Application/"]
COPY ["Services/Users/Users.Infrastructure/Users.Infrastructure.csproj", "./Services/Users/Users.Infrastructure/"]
RUN dotnet restore "./Client/eShopping.Client/eShopping.Client.csproj" -r $BUILD_PLATFORM --disable-parallel
RUN dotnet restore "./Services/Basket/Basket.API/Basket.API.csproj" -r $BUILD_PLATFORM --disable-parallel
RUN dotnet restore "./Services/Catalog/Catalog.API/Catalog.API.csproj" -r $BUILD_PLATFORM --disable-parallel
RUN dotnet restore "./Services/Users/Users.Api/Users.Api.csproj" -r $BUILD_PLATFORM --disable-parallel

# BUILD
FROM restorer AS buildUsers
WORKDIR "/src"
COPY . .
RUN dotnet build "./Services/Users/Users.Api/Users.Api.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/build/Users

FROM restorer AS buildBasket
WORKDIR "/src"
COPY . .
RUN dotnet build "./Services/Basket/Basket.API/Basket.API.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/build/Basket

FROM restorer AS buildCatalog
WORKDIR "/src"
COPY . .
RUN dotnet build "./Services/Catalog/Catalog.API/Catalog.API.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/build/Catalog

FROM restorer AS buildClient
WORKDIR "/src"
COPY . .
RUN dotnet build "./Client/eShopping.Client/eShopping.Client.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/build/Client

# PUBLISH
FROM buildUsers as publishUsers
WORKDIR "/src"
RUN dotnet publish "./Services/Users/Users.Api/Users.Api.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/publish/Users /p:UseAppHost=false 

FROM buildBasket as publishBasket
WORKDIR "/src"
RUN dotnet publish "./Services/Basket/Basket.API/Basket.API.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/publish/Basket /p:UseAppHost=false 

FROM buildCatalog as publishCatalog
WORKDIR "/src"
RUN dotnet publish "./Services/Catalog/Catalog.API/Catalog.API.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/publish/Catalog /p:UseAppHost=false 

FROM buildClient as publishClient
WORKDIR "/src"
RUN dotnet publish "./Client/eShopping.Client/eShopping.Client.csproj" -c $BUILD_CONFIGURATION -r $BUILD_PLATFORM --self-contained false -o /app/publish/Client /p:UseAppHost=false 

FROM base AS final
WORKDIR /app
COPY --from=publishUsers /app/publish/Users ./Users
COPY --from=publishBasket /app/publish/Basket ./Basket
COPY --from=publishCatalog /app/publish/Catalog ./Catalog
COPY --from=publishClient /app/publish/Client ./Client
COPY wrapper_script.sh /app/wrapper_script.sh

RUN ["chmod", "+x", "/app/wrapper_script.sh"]

CMD ["sh", "/app/wrapper_script.sh"]