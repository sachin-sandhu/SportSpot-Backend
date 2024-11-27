# SportSpot

## About

SportSpot is a web application designed to manage sports-related activities. It includes various services and components such as a backend, database services, and a REST emulator.

## Services

### Backend

The backend service is executed in a Docker container and can be accessed at [http://localhost:8080](http://localhost:8080).

### MongoDB

The MongoDB service is executed in a Docker container and can be accessed at [http://localhost:27017](http://localhost:27017).

### MariaDB

The MariaDB service is executed in a Docker container and can be reached at [http://localhost:8082](http://localhost:8082).  
Username: `root`  
Password: `secret`

## Requirements

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)
- [DotNet 9](https://dotnet.microsoft.com/en-us/download/dotnet)

## Installation

1. Clone the Repository:

   ```bash
   git clone https://github.com/PlaySkyHD/SportSpot-Backend.git
   cd SportSpot-Backend
   ```

2. Create and start the Docker-Container:

   ```bash
   docker-compose up --build -d
   ```

3. Open any browser and open the Swagger-UI:  
   [http://localhost:8080/swagger](http://localhost:8080/swagger)

## Database

1. **MongoDB**

   - Dev-UI: [http://localhost:8081](http://localhost:8081)

2. **MariaDB**
   - Dev-UI: [http://localhost:8082](http://localhost:8082)  
     Username: `root`  
     Password: `secret`

## Tests

### Requirements

- DotNet 9
- Docker

### Unit-Tests

**Command:**

```bash
dotnet test SportSpot-Test/SportSpot-Test.csproj -v d
```

### Integration-Tests

**You must execute all command with Powershell.**

#### Without OAuth

**Command:**

1. Create Dummy-EnviormentVariable:
   ```powershell
   Set-Content -Path Subscription.env -Value AZURE_MAPS_SUBSCRIPTION_KEY=Dummy
   ```
2. Start Docker-Container:
   ```powershell
   docker compose up --build -d
   ```
3. Execute Test:
   ```powershell
   dotnet test Integration-Test/Integration-Test.csproj -v d
   ```

#### With OAuth

**Command:**

1. Create Dummy-Environment variable:
   ```powershell
   Set-Content -Path Subscription.env -Value AZURE_MAPS_SUBSCRIPTION_KEY=Dummy
   ```
1. Change Environment Variable to execute oauth tests:
   ```powershell
   (Get-Content Development.env) -replace '^OAUTH_GOOGLE_USER_INFORMATION_ENDPOINT=.*', 'OAUTH_GOOGLE_USER_INFORMATION_ENDPOINT=http://restemulator:8083/oauth' | Set-Content Development.env
   ```
1. Start Docker Container:
   ```powershell
   docker compose up --build -d
   ```
1. Set OAuth-Environment variable and Execute the tests:
   ```powershell
   $env:RUN_OAUTH_TEST="true"
   dotnet test Integration-Test/Integration-Test.csproj -v d
   ```

## License

This project is released under the MIT License. For more information, see the \`LICENSE` file.
