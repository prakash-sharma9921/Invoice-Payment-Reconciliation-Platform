# Commands log — build steps and what they do

A running reference of the commands used to build this project, so any
future project can follow the same path faster.

Each command lives inside a fenced code block so it renders correctly on
GitHub (comments stay as comments, and each command stays on its own line).

## One-time machine setup (verify tools)

```bash
git --version
dotnet --list-sdks          # confirm .NET 10 SDK present
dotnet ef --version         # confirm EF Core CLI tool
node -v ; npm -v            # confirm Node.js + npm
docker --version            # confirm Docker

# Install EF Core CLI tool (once per machine):
dotnet tool install --global dotnet-ef
```

## Create the solution and projects (run inside backend/)

```bash
dotnet new sln --name Recon   # .slnx by default on .NET 10

dotnet new classlib -n Recon.Domain         -o src/Recon.Domain
dotnet new classlib -n Recon.Application     -o src/Recon.Application
dotnet new classlib -n Recon.Infrastructure  -o src/Recon.Infrastructure
dotnet new webapi    -n Recon.Api            -o src/Recon.Api
dotnet new xunit -n Recon.UnitTests        -o tests/Recon.UnitTests
dotnet new xunit -n Recon.IntegrationTests -o tests/Recon.IntegrationTests
```

## Add projects to the solution

```bash
dotnet sln add src/Recon.Domain src/Recon.Application src/Recon.Infrastructure src/Recon.Api
dotnet sln add tests/Recon.UnitTests tests/Recon.IntegrationTests
```

## Connect projects (references point inward)

```bash
dotnet add src/Recon.Application    reference src/Recon.Domain
dotnet add src/Recon.Infrastructure reference src/Recon.Application
dotnet add src/Recon.Api            reference src/Recon.Application
dotnet add src/Recon.Api            reference src/Recon.Infrastructure
dotnet add tests/Recon.UnitTests        reference src/Recon.Domain
dotnet add tests/Recon.UnitTests        reference src/Recon.Application
dotnet add tests/Recon.IntegrationTests reference src/Recon.Api
```

## Add EF Core packages

```bash
dotnet add src/Recon.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/Recon.Api            package Microsoft.EntityFrameworkCore.Design
dotnet add src/Recon.Api            package Microsoft.EntityFrameworkCore.SqlServer
```

## Application-layer packages (validation + EF Core abstractions)

```bash
dotnet add src/Recon.Application package Microsoft.EntityFrameworkCore   # DbSet + async LINQ
dotnet add src/Recon.Application package FluentValidation                # input validation rules
dotnet add src/Recon.Api         package FluentValidation.DependencyInjectionExtensions
```

## Build

```bash
dotnet build
```

## Database migrations (run from backend/)

```bash
# Create a migration (writes code only; DB not needed yet):
dotnet ef migrations add InitialCreate --project src/Recon.Infrastructure --startup-project src/Recon.Api

# Apply migrations to the database (DB must be running):
dotnet ef database update --project src/Recon.Infrastructure --startup-project src/Recon.Api
```

Reminder on the two flags:

- `--project` = where the migration/DbContext code lives (Infrastructure).
- `--startup-project` = which app to boot to read the connection string (Api).

Note: do not rename or delete a migration after it has been applied — it is
permanent history that EF Core tracks. (Renaming its namespace to keep the code
consistent is fine, as long as everything still builds.)

## Docker (run from backend/, where docker-compose.yml is)

```bash
docker compose up -d          # start SQL Server container in the background
docker compose ps             # check status (look for "Up" and port 14330)
docker compose logs sqlserver # see why it crashed, if it did
docker compose stop           # stop and keep the container
docker compose down           # stop and remove the container (keeps data volume)
docker compose down -v        # also delete the data volume (wipes the database)
```

## Peek into the DB from the container (no SSMS)

```bash
docker exec -it recon-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "Recon_Local_Pass123" -C -Q "SELECT name FROM sys.tables;"
```

## Connect SSMS to the container

```text
Server name: localhost,14330    (comma before the port, not a colon)
Authentication: SQL Server Authentication
Login: sa
Password: <the password you set in .env>
Tick "Trust server certificate"
```

## Secrets setup (per machine — values are NOT stored in Git)

```bash
# 1. Local database password (for the Docker container).
#    Copy the template, then edit .env and set a password.
#    Windows (PowerShell), run from backend/:
Copy-Item .env.example .env

# 2. API connection string (stored in your Windows user profile, not the repo):
dotnet user-secrets init --project src/Recon.Api
dotnet user-secrets set "ConnectionStrings:ReconDb" "Server=localhost,14330;Database=ReconDb;User Id=sa;Password=YOUR_PASSWORD_HERE;TrustServerCertificate=True;" --project src/Recon.Api

# See what secrets are stored:
dotnet user-secrets list --project src/Recon.Api
```

## Run the API

```bash
dotnet run --project src/Recon.Api
# then use the .http file (REST Client extension) to call the endpoints
```

## Testing

```bash
# Run every test in the solution (from backend/)
dotnet test

# Run only one test project
dotnet test tests/Recon.UnitTests
dotnet test tests/Recon.IntegrationTests
```

Packages used for testing:

- Unit tests: `Microsoft.EntityFrameworkCore.InMemory` (fake in-memory database),
  `FluentValidation` (validator test helpers).
- Integration tests: `Microsoft.AspNetCore.Mvc.Testing` (WebApplicationFactory,
  in-memory host), `Microsoft.EntityFrameworkCore.InMemory` (swap-in DB for tests).

Note: tests do NOT need Docker/SQL Server running — both use in-memory databases.
Docker + SQL Server are only needed to RUN the app (`dotnet run`), not to test it.

## Git (two accounts on one machine)

```bash
# Set the commit identity for THIS repo only (no --global), so a personal repo
# is not stamped with a work identity:
git config user.name "Your Name"
git config user.email "your-personal-email@example.com"

# Verify what will be used:
git config user.name
git config user.email

# Check which account actually pushes (login, stored in Windows Credential Manager):
git credential fill
# then type these two lines and press Enter twice:
#   protocol=https
#   host=github.com
# the "username=" line it prints is the account your pushes use.

# Normal commit + push flow:
git status
git add .
git commit -m "message"
git push origin master
```
