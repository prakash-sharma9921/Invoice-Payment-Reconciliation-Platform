# Adding a new entity — the recipe

A repeatable, end-to-end checklist for adding a new entity (Client, Invoice,
Bank, …) to the platform. Follow the steps in order.

`Tenant` is used as the running example, because it is the first entity we
actually built — so you can compare this recipe against real files in the repo.
For a new entity, replace `Tenant` with your entity name (e.g. `Client`).

---

## The two directions to keep in mind

**Dependency direction** (who is allowed to know about whom) points _inward_:

```
Domain  <-  Application  <-  Infrastructure
                ^                 ^
                |                 |
                +------- Api -----+
```

- Domain knows nobody.
- Application knows Domain.
- Infrastructure knows Application (it _implements_ Application's interface).
- Api knows Application + Infrastructure.

**Request flow** (how a live request travels) goes top-to-bottom and back:

```
HTTP request
  -> Endpoint (Api)
  -> Validator (Application)          checks the input
  -> Service (Application)            the logic
  -> IApplicationDbContext            the interface
  -> ReconDbContext (Infrastructure)  the real database bridge
  -> SQL Server (Docker)
  <- data returns, Mapping turns entity into response, endpoint returns JSON
```

The trick that makes both work: Application _defines_ `IApplicationDbContext`;
Infrastructure _implements_ it. So at runtime Infrastructure supplies the
database, but at compile time the dependency still points inward.

---

## Project references and packages (already set up once)

| Project        | References                  | Key packages                                                                      |
| -------------- | --------------------------- | --------------------------------------------------------------------------------- |
| Domain         | (nothing)                   | (none)                                                                            |
| Application    | Domain                      | EF Core (abstractions), FluentValidation                                          |
| Infrastructure | Application                 | EF Core SqlServer                                                                 |
| Api            | Application, Infrastructure | EF Core Design, FluentValidation.DependencyInjectionExtensions, EF Core SqlServer |

---

## Step 1 — Domain: the entity

File: `src/Recon.Domain/Entities/Tenant.cs`

- Plain C# class, just properties.
- No database attributes, no logic about saving.
- Every tenant-owned entity carries a `TenantId`. (The `Tenant` entity itself is
  the exception — it does NOT have a `TenantId`, because it _is_ the tenant.)

---

## Step 2 — Application: expose it on the DB interface

File: `src/Recon.Application/Common/Interfaces/IApplicationDbContext.cs`

Add one line:

```csharp
DbSet<Tenant> Tenants { get; }
```

---

## Step 3 — Infrastructure: table + rules

- In `src/Recon.Infrastructure/Persistence/ReconDbContext.cs` add:

  ```csharp
  public DbSet<Tenant> Tenants => Set<Tenant>();
  ```

  (This satisfies the interface line from Step 2.)

- Create `src/Recon.Infrastructure/Persistence/Configurations/TenantConfiguration.cs`
  using the **EF Core Fluent API** (`IEntityTypeConfiguration<Tenant>`): table
  name, key, required columns, max lengths, unique indexes.
  It is picked up automatically by `ApplyConfigurationsFromAssembly` — no manual
  wiring.

---

## Step 4 — Migration: build the table

Run from `backend/`:

```bash
# writes migration code describing the new/changed tables
dotnet ef migrations add AddTenant --project src/Recon.Infrastructure --startup-project src/Recon.Api

# applies it to the running database
dotnet ef database update --project src/Recon.Infrastructure --startup-project src/Recon.Api
```

Reminder on the two flags:

- `--project` = where the migration/DbContext code lives (Infrastructure).
- `--startup-project` = which app to boot to read the connection string (Api).

Do not rename or delete a migration once it has been applied — it is permanent
history EF Core tracks.

---

## Step 5 — Application: contracts, validator, service (3 files)

Folder: `src/Recon.Application/Tenants/`

**`TenantContracts.cs`** — request DTO(s) + response DTO + hand-written mapping.
(Use `record` for DTOs; `class` for things with behaviour.)

```csharp
public record CreateTenantRequest(string Name, string Code);

public record TenantResponse(
    Guid Id, string Name, string Code, DateTime CreatedAt, bool IsActive);

public static class TenantMapping
{
    // Hand-written on purpose: explicit, easy to read, easy to debug.
    public static TenantResponse ToResponse(this Tenant tenant) =>
        new(tenant.Id, tenant.Name, tenant.Code, tenant.CreatedAt, tenant.IsActive);
}
```

**`TenantValidator.cs`** — FluentValidation rules.

Naming convention: **one validator FILE per entity** (`TenantValidator.cs`),
holding a **separate CLASS per action** inside it — `CreateTenantValidator`,
`UpdateTenantValidator`, and so on. Each validator class targets **its own
request type** (create and update carry different data, so they need different
request records). Grouping them in one file keeps each entity's rules together;
the assembly scan still registers every class regardless of file name.

```csharp
public class CreateTenantValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantValidator(IApplicationDbContext db)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);

        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(50)
            .Matches("^[a-z0-9-]+$")
            .MustAsync(async (code, ct) =>
                !await db.Tenants.AnyAsync(t => t.Code == code, ct))
                .WithMessage("A tenant with this code already exists.");
    }
}

// When update is needed later, add another class in the SAME file:
// public class UpdateTenantValidator : AbstractValidator<UpdateTenantRequest> { ... }
```

**`TenantService.cs`** — interface + implementation together.

```csharp
public interface ITenantService
{
    Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken ct = default);
}

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _db;
    public TenantService(IApplicationDbContext db) => _db = db;
    // CreateAsync / GetAllAsync using _db + ToResponse()
}
```

Notes on types used here:

- `Task<T>` — for methods that do I/O (database, network, files). Pure in-memory
  work (like `ToResponse`) stays synchronous.
- `IReadOnlyList<T>` — default return for a list the caller only reads. Use a
  nullable single (`TenantResponse?`) for "get one by id"; a paged type later.
- `CancellationToken` — accept it at every async level and pass it down, so a
  cancelled request (client disconnects, timeout) stops work early.

---

## Step 6 — Api: the endpoints

File: `src/Recon.Api/Endpoints/TenantEndpoints.cs`

```csharp
public static class TenantEndpoints
{
    public static void MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tenants").WithTags("Tenants");

        group.MapGet("/", async (ITenantService service, CancellationToken ct) =>
            Results.Ok(await service.GetAllAsync(ct)));

        group.MapPost("/", async (
            CreateTenantRequest request,
            IValidator<CreateTenantRequest> validator,
            ITenantService service,
            CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(request, ct);
            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var created = await service.CreateAsync(request, ct);
            return Results.Created($"/api/tenants/{created.Id}", created);
        });
    }
}
```

The validator is **called here, in the endpoint**, before the service runs.

---

## Step 7 — Program.cs: wire the two new things

```csharp
builder.Services.AddScoped<ITenantService, TenantService>();   // register the service
// ...
app.MapTenantEndpoints();                                      // register the endpoints
```

You do **not** register validators one by one —
`AddValidatorsFromAssemblyContaining<...>` scans the whole Application project
and finds every validator automatically. (The type in its angle brackets is only
a marker pointing at the project to scan — any type from that project works.)

---

## Step 8 — Test

- Unit tests (`tests/Recon.UnitTests`): test the service and validators with the
  EF Core InMemory provider (a fake DB), no web server.
- Integration tests (`tests/Recon.IntegrationTests`): boot the real API with
  `WebApplicationFactory`, swap SQL Server for InMemory, and call the endpoints
  over HTTP.
- Also add requests to a `.http` file for manual checking (REST Client).
- Run everything: `dotnet test`.

---

## File-purpose quick reference

| File                       | Layer          | Purpose                                      |
| -------------------------- | -------------- | -------------------------------------------- |
| `Tenant.cs`                | Domain         | What the thing is (properties only)          |
| `IApplicationDbContext.cs` | Application    | The DB "capability" the service depends on   |
| `TenantConfiguration.cs`   | Infrastructure | How the table is shaped (Fluent API)         |
| `ReconDbContext.cs`        | Infrastructure | The real database bridge                     |
| `TenantContracts.cs`       | Application    | Request DTO(s) + response DTO + mapping      |
| `TenantValidator.cs`       | Application    | Input rules — one file, one class per action |
| `TenantService.cs`         | Application    | Interface + logic                            |
| `TenantEndpoints.cs`       | Api            | HTTP endpoints; calls validator then service |

---

## Two "fluent" names not to confuse

- **FluentValidation** (`AbstractValidator<T>`) — checks user **input**. Application layer.
- **EF Core Fluent API** (`IEntityTypeConfiguration<T>`) — shapes the **database table**. Infrastructure layer.

---

## Manual vs automatic (what you must wire each time)

- Automatic: validators (assembly scan), table configurations
  (`ApplyConfigurationsFromAssembly`).
- Manual each time: the `DbSet` on the interface + context, the service
  registration in `Program.cs`, the `app.Map...Endpoints()` call, and the
  migration commands.
