FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
WORKDIR /app
EXPOSE 5207

ENV ASPNETCORE_URLS=http://+:5207

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:11.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["AspNetWithDocker.csproj", "./"]
RUN dotnet restore "AspNetWithDocker.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AspNetWithDocker.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "AspNetWithDocker.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspNetWithDocker.dll"]
