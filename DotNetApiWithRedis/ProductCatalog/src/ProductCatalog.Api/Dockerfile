FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5134
ENV ASPNETCORE_URLS=http://+:5134

USER $APP_UID

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG configuration=Release
WORKDIR /src

COPY [ "ProductCatalog.Api.csproj", "./" ]

RUN dotnet restore "ProductCatalog.Api.csproj"

COPY . .

RUN dotnet build "ProductCatalog.Api.csproj" -c $configuration -o /app/build    

FROM build AS publish
ARG configuration=Release

RUN dotnet publish "ProductCatalog.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ProductCatalog.Api.dll"]