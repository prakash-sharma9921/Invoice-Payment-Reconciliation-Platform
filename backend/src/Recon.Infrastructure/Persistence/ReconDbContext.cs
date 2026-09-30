using Microsoft.EntityFrameworkCore;
using Recon.Application.Common.Interfaces;
using Recon.Domain.Entities;

namespace Recon.Infrastructure.Persistence;

public class ReconDbContext : DbContext, IApplicationDbContext
{
    public ReconDbContext(DbContextOptions<ReconDbContext> options) : base(options)
    {

    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReconDbContext).Assembly);
    }
}