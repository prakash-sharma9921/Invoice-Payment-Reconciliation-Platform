
using Recon.Domain.Entities;

namespace Recon.Application.Clients;

public record CreateClientRequest(string Name, string? Email, string? Phone, ClientType Type);

public record ClientResponse
(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Email,
    string? Phone,
    ClientType Type,
    bool IsActive,
    DateTime CreatedAt
);

public static class ClientMapping
{
    public static ClientResponse ToResponse(this Client client) =>
        new(client.Id, client.TenantId, client.Name, client.Email, client.Phone, client.Type, client.IsActive, client.CreatedAt);
}