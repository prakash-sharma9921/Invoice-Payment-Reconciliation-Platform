using Recon.Domain.Entities;

namespace Recon.Application.Tenants;

// A "record" is a short way to declare a small data-holder class.
// This is what the API receives when someone creates a tenant.
public record CreateTenantRequest(string Name, string Code);

public record TenantResponse(
    Guid Id,
    string Name,
    string Code,
    DateTime CreatedAt,
    bool IsActive
);

public static class TenantMapping
{
    // Converts the database entity into the outward-facing response.
    // Written by hand on purpose: explicit, easy to read, easy to debug.
    public static TenantResponse ToResponse(this Tenant tenant) =>
        new(tenant.Id, tenant.Name, tenant.Code, tenant.CreatedAt, tenant.IsActive);
}