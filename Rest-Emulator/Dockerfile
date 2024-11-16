# Base Stage for general settings that never change
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Build Stage with Dotnet SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY *.csproj ./Rest-Emulator/
RUN dotnet restore Rest-Emulator/Rest-Emulator.csproj

# copy everything else and build app
COPY . ./Rest-Emulator/
WORKDIR /src/Rest-Emulator
RUN dotnet publish -c Release -o /app

# final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Rest-Emulator.dll"]
USER 1000