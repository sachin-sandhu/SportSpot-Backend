
# SportSpot

## Übersicht

SportSpot ist ein Backend-Projekt, das mit verschiedenen Microservices arbeitet. Es bietet Funktionalitäten zur Verwaltung und Interaktion über eine API, unterstützt durch Docker-Container.

## Voraussetzungen

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [DotNet 8](https://dotnet.microsoft.com/en-us/download/dotnet)

## Installation

1. Klone das Repository:
   ```bash
   git clone https://github.com/PlaySkyHD/SportSpot-Backend.git
   cd SportSpot-Backend
   ```

2. Erstelle und starte die Docker-Container:
   ```bash
   docker-compose up --build -d
   ```

3. Öffne deinen Browser und rufe die Swagger-Oberfläche auf:  
   [http://localhost:8080/swagger](http://localhost:8080/swagger)

## Datenbanken

1. **MongoDB**
   - Entwicklungs-UI: [http://localhost:8081](http://localhost:8081)

2. **MariaDB**
   - Entwicklungs-UI: [http://localhost:8082](http://localhost:8082)  
     Benutzername: `root`  
     Passwort: `secret`

## Tests ausführen

### Voraussetzungen
- DotNet 8
- Docker

### Unit-Tests
**Befehl:**
```bash
dotnet test SportSpot-Test/SportSpot-Test.csproj -v d
```

### Integration-Tests  
**Alle Befehle müssen in PowerShell ausgeführt werden.**

#### Ohne OAuth
**Befehle:**
1. Erstelle eine Dummy-Umgebungsvariable:  
   ```powershell
   Set-Content -Path Subscription.env -Value AZURE_MAPS_SUBSCRIPTION_KEY=Dummy
   ```
2. Starte die Docker-Container:  
   ```powershell
   docker compose up --build -d
   ```
3. Führe die Tests aus:  
   ```powershell
   dotnet test Integration-Test/Integration-Test.csproj -v d
   ```

#### Mit OAuth
**Befehle:**
1. Erstelle eine Dummy-Umgebungsvariable:  
   ```powershell
   Set-Content -Path Subscription.env -Value AZURE_MAPS_SUBSCRIPTION_KEY=Dummy
   ```
2. Ändere die Entwicklungsumgebung, um OAuth-Tests zu aktivieren:  
   ```powershell
   (Get-Content Development.env) -replace '^OAUTH_GOOGLE_USER_INFORMATION_ENDPOINT=.*', 'OAUTH_GOOGLE_USER_INFORMATION_ENDPOINT=http://restemulator:8083/oauth' | Set-Content Development.env
   ```
3. Starte die Docker-Container:  
   ```powershell
   docker compose up --build -d
   ```
4. Setze die OAuth-Umgebungsvariable und führe die Tests aus:  
   ```powershell
   $env:RUN_OAUTH_TEST="true"
   dotnet test Integration-Test/Integration-Test.csproj -v d
   ```

## Lizenz

Dieses Projekt ist unter der MIT-Lizenz verrüffentlicht. Weitere Informationen findest du in der Datei \`LICENSE`.