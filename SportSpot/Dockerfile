# Base Stage for general settings that never change
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build Stage with Dotnet SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.csproj ./SportSpot/
RUN dotnet restore SportSpot/SportSpot.csproj

# copy everything else and build app
COPY . ./SportSpot/
WORKDIR /src/SportSpot
RUN dotnet publish -c Release -o /app

# final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "SportSpot.dll"]
USER 1000