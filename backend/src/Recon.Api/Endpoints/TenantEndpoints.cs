using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Recon.Application.Tenants;

namespace Recon.Api.Endpoints;

public static class TenantEndpoints
{
    public static void MapTenantEndpoints(this IEndpointRouteBuilder app)
    {
        // All tenant endpoints share the /api/tenants path prefix.
        var group = app.MapGroup("/api/tenants").WithTags("Tenants");

        // GET /api/tenants  -> list all tenants
        group.MapGet("/", async (ITenantService service, CancellationToken ct) =>
        {
            var tenants = await service.GetAllAsync(ct);
            return Results.Ok(tenants);
        });

        // POST /api/tenants -> create a tenant
        group.MapPost("/", async (
            CreateTenantRequest request,
            IValidator<CreateTenantRequest> Validator,
            ITenantService service,
            CancellationToken ct) =>
        {
            var result = await Validator.ValidateAsync(request, ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());  // 400 with messages

            var created = await service.CreateAsync(request, ct);
            return Results.Created($"/api/tenants/{created.Id}", created);  // 201 Created
        });
    }
}