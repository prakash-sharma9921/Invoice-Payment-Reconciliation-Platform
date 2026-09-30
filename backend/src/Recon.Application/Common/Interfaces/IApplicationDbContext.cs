using Microsoft.EntityFrameworkCore;
using Recon.Domain.Entities;

namespace Recon.Application.Common.Interfaces;

// The Application talks to the database ONLY through this small interface.
// Infrastructure's ReconDbContext will implement it. This keeps the
// dependency pointing inward (Application knows nothing about SQL Server).
public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}