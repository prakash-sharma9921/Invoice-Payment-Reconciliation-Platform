using Microsoft.EntityFrameworkCore;
using Recon.Application.Common.Interfaces;
using Recon.Domain.Entities;

namespace Recon.Application.Tenants;

public interface ITenantService
{
    Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default);
}

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _db;

    // The database interface is handed to us automatically (dependency injection).
    public TenantService(IApplicationDbContext db) => _db = db;

    public async Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _db.Tenants.Add(tenant);
        await _db.SaveChangesAsync(cancellationToken);

        return tenant.ToResponse();
    }

    public async Task<IReadOnlyList<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _db.Tenants.OrderBy(t => t.Name).ToListAsync(cancellationToken);

        return tenants.Select(t => t.ToResponse()).ToList();
    }
}