# SportSpot

## Übersicht

Eine kurze Beschreibung des Projekts und seiner Funktionalität.

## Voraussetzungen

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Installation

1. Klone das Repository:
   https://github.com/PlaySkyHD/SportSpot-Backend.git
   cd SportSpot-Backend

2. Erstelle und starte die Docker-Container:
   docker-compose up --build -d

3. Öffne deinen Browser und gehe zu `http://localhost:8080/swagger`

## Datenbanken

1. MongoDB:
  - Dev-UI: http://localhost:8081
2. MariaDB:
  - Dev-UI: http://localhost:8082 (Username: root & Password: secret)

## Tests ausführen

### Requirements
1. DotNet 8
2. Docker

### Unit-Test
**Commands:**
1. dotnet test SportSpot-Test/SportSpot-Test.csproj -v d

### Integration-Test

**All commands must be executed with Powershell.**

### With OAuth
**Commands:**
1. Set-Content -Path Subscription.env -Value AZURE_MAPS_SUBSCRIPTION_KEY=Dummy
2. (Get-Content Development.env) -replace '^OAUTH_GOOGLE_USER_INFORMATION_ENDPOINT=.*', 'OAUTH_GOOGLE_USER_INFORMATION_ENDPOINT=http://restemulator:8083/oauth' | Set-Content Development.env
2. docker compose up --build -d
3. ($env:RUN_OAUTH_TEST="true") | dotnet test Integration-Test/Integration-Test.csproj -v d

#### Without OAuth
**Commands:**
1. Set-Content -Path Subscription.env -Value AZURE_MAPS_SUBSCRIPTION_KEY=Dummy
2. docker compose up --build -d
3. dotnet test Integration-Test/Integration-Test.csproj -v d

## Lizenz

Dieses Projekt ist unter der MIT-Lizenz lizenziert. Weitere Informationen findest du in der `LICENSE`-Datei.
