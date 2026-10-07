using FluentValidation;
using Recon.Application.Clients;

namespace Recon.Api.Endpoints;

public static class ClientEndpoints
{
    public static void MapClientEndpoints(this IEndpointRouteBuilder app)
    {
        // All client endpoints share the /api/clients path prefix.
        var group = app.MapGroup("/api/clients").WithTags("Clients");

        // GET /api/clients  -> list all clients
        group.MapGet("/", async (IClientService service, CancellationToken ct) =>
        {
            var clients = await service.GetAllAsync(ct);

            return Results.Ok(clients);
        });

        // POST /api/clients -> create a client
        group.MapPost("/", async (
            CreateClientRequest request,
            IValidator<CreateClientRequest> validator,
            IClientService service,
            CancellationToken ct) =>
        {
            var result = await validator.ValidateAsync(request, ct);

            if (!result.IsValid)
                return Results.ValidationProblem(result.ToDictionary());

            var created = await service.CreateAsync(request, ct);

            return Results.Created($"/api/clients/{created.Id}", created);

        });
    }
}