using Microsoft.EntityFrameworkCore;
using Recon.Application.Common.Interfaces;
using Recon.Domain.Entities;

namespace Recon.Application.Clients;

public interface IClientService
{
    Task<ClientResponse> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClientResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}

public class ClientService : IClientService
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentTenant _tenant;

    public ClientService(IApplicationDbContext db, ICurrentTenant tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    public async Task<ClientResponse> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            TenantId = _tenant.TenantId,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Type = request.Type,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync(cancellationToken);

        return client.ToResponse();
    }

    public async Task<IReadOnlyList<ClientResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var clients = await _db.Clients.OrderBy(c => c.Name).ToListAsync(cancellationToken);

        return clients.Select(c => c.ToResponse()).ToList();
    }
}