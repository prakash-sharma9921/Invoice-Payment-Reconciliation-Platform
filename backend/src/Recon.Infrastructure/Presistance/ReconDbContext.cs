using Microsoft.EntityFrameworkCore;
using Recon.Domain.Entities;

namespace Recon.Infrastructure.Presistance;

public class ReconDbContext : DbContext
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