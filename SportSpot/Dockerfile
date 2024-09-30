# Base Stage for general settings that never change
FROM mcr.microsoft.com/dotnet/aspnet:8.0@sha256:30d8619d9a4f68508d9b17fc2088e857e629d3f9ceaaf57c22d6747f7326d89e AS base
WORKDIR /app
EXPOSE 8080

# Build Stage with Dotnet SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:df188b6cb642450d83d3d857a992bbbef248212fd456dcb9ae582ddd1bb96dce AS build
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