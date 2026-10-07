using Microsoft.EntityFrameworkCore;
using Recon.Application.Common.Interfaces;
using Recon.Domain.Entities;

namespace Recon.Infrastructure.Persistence;

public class ReconDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentTenant _currentTenant;
    public ReconDbContext(DbContextOptions<ReconDbContext> options, ICurrentTenant currentTenant) : base(options)
    {
        _currentTenant = currentTenant;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Client> Clients => Set<Client>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReconDbContext).Assembly);

        // THE GLOBAL QUERY FILTER:
        // every query on Clients is automatically limited to the current tenant.
        modelBuilder.Entity<Client>().HasQueryFilter(c => c.TenantId == _currentTenant.TenantId);
    }
}