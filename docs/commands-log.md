# Commands log — build steps and what they do

A running reference of the commands used to build this project, so any
future project can follow the same path faster.

## One-time machine setup (verify tools)

git --version
dotnet --list-sdks # confirm .NET 10 SDK present
dotnet ef --version # confirm EF Core CLI tool
node -v ; npm -v # confirm Node.js + npm
docker --version # confirm Docker

Install EF Core CLI tool (once per machine):
dotnet tool install --global dotnet-ef

## Create the solution and projects (run inside backend/)

dotnet new sln --name Recon # .slnx by default on .NET 10

dotnet new classlib -n Recon.Domain -o src/Recon.Domain
dotnet new classlib -n Recon.Application -o src/Recon.Application
dotnet new classlib -n Recon.Infrastructure -o src/Recon.Infrastructure
dotnet new webapi -n Recon.Api -o src/Recon.Api
dotnet new xunit -n Recon.UnitTests -o tests/Recon.UnitTests
dotnet new xunit -n Recon.IntegrationTests -o tests/Recon.IntegrationTests

## Add projects to the solution

dotnet sln add src/Recon.Domain src/Recon.Application src/Recon.Infrastructure src/Recon.Api
dotnet sln add tests/Recon.UnitTests tests/Recon.IntegrationTests

## Connect projects (references point inward)

dotnet add src/Recon.Application reference src/Recon.Domain
dotnet add src/Recon.Infrastructure reference src/Recon.Application
dotnet add src/Recon.Api reference src/Recon.Application
dotnet add src/Recon.Api reference src/Recon.Infrastructure
dotnet add tests/Recon.UnitTests reference src/Recon.Domain
dotnet add tests/Recon.UnitTests reference src/Recon.Application
dotnet add tests/Recon.IntegrationTests reference src/Recon.Api

## Add EF Core packages

dotnet add src/Recon.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/Recon.Api package Microsoft.EntityFrameworkCore.Design
dotnet add src/Recon.Api package Microsoft.EntityFrameworkCore.SqlServer

## Build

dotnet build

## Database migrations (run from backend/)

# Create a migration (writes code only; DB not needed yet):

dotnet ef migrations add InitialCreate --project src/Recon.Infrastructure --startup-project src/Recon.Api

# Apply migrations to the database (DB must be running):

dotnet ef database update --project src/Recon.Infrastructure --startup-project src/Recon.Api

## Docker (run from backend/, where docker-compose.yml is)

docker compose up -d # start SQL Server container in the background
docker compose ps # check status (look for "Up" and port 14330)
docker compose logs sqlserver # see why it crashed, if it did
docker compose down # stop and remove the container (keeps data volume)
docker compose down -v # also delete the data volume (wipes the database)

## Peek into the DB from the container (no SSMS)

docker exec -it recon-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Recon_Local_Pass123" -C -Q "SELECT name FROM sys.tables;"

## Connect SSMS to the container

# Server name: localhost,14330 (comma before port)

# Auth: SQL Server Authentication, Login: sa, Password: Recon_Local_Pass123

# Tick "Trust server certificate"

## Secrets setup (per machine — values are NOT stored in Git)

# 1. Local database password (for the Docker container):

# Copy the template, then edit .env and set a password.

# Windows (PowerShell), run from backend/:

Copy-Item .env.example .env

# 2. API connection string (stored in your Windows user profile, not the repo):

dotnet user-secrets init --project src/Recon.Api
dotnet user-secrets set "ConnectionStrings:ReconDb" "Server=localhost,14330;Database=ReconDb;User Id=sa;Password=YOUR_PASSWORD_HERE;TrustServerCertificate=True;" --project src/Recon.Api

# See what secrets are stored:

dotnet user-secrets list --project src/Recon.Api
