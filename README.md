# Invoice & Payment Reconciliation Platform

A multi-tenant platform that manages invoices, ingests payments from multiple
channels, and automatically matches payments against invoices — including
partial payments, overpayments, and one payment settling several invoices.

> **Status:** in active development. Being built one vertical slice at a time
> (database → domain → API → UI → tests). Foundation complete: solution
> structure, EF Core, and the first table are in place.

## Tech stack

- **Backend:** ASP.NET Core (.NET 10), C#, EF Core (code-first), SQL Server
- **Architecture:** layered — Domain / Application / Infrastructure / API
- **Frontend:** React + Vite + TypeScript + Tailwind (coming in a later slice)
- **Testing:** xUnit, WebApplicationFactory
- **Local dev:** SQL Server in Docker
- **Multi-tenancy:** shared database, TenantId column, EF Core query filter

## Getting started (local)

**Prerequisites:** .NET 10 SDK · Docker Desktop · the EF Core CLI
(`dotnet tool install --global dotnet-ef`)

```bash
# 1. Set the local database password
cd backend
Copy-Item .env.example .env        # then open .env and set a password

# 2. Set the API connection string (use the SAME password)
dotnet user-secrets init --project src/Recon.Api
dotnet user-secrets set "ConnectionStrings:ReconDb" "Server=localhost,14330;Database=ReconDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True;" --project src/Recon.Api

# 3. Start the database
docker compose up -d

# 4. Create the database tables
dotnet ef database update --project src/Recon.Infrastructure --startup-project src/Recon.Api
```

## Project documentation

- `docs/decisions/` — why each significant choice was made (ADRs)
- `docs/commands-log.md` — the commands used to build the project

## License

MIT — see [LICENSE](./LICENSE).
