FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5001
RUN mkdir -p logs

EXPOSE 5001
EXPOSE 5002
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Spotify.Identity/Spotify.Identity.csproj", "Spotify.Identity/"]
RUN dotnet restore "Spotify.Identity/Spotify.Identity.csproj"
COPY . .
WORKDIR "/src/Spotify.Identity"
RUN dotnet build "Spotify.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Spotify.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spotify.Identity.dll"]
