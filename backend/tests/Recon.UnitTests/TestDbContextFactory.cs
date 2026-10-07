using Microsoft.EntityFrameworkCore;
using Recon.Infrastructure.Persistence;

namespace Recon.UnitTests;

public static class TestDbContextFactory
{
    public static ReconDbContext Create(Guid tenantId)
    {
        var options = new DbContextOptionsBuilder<ReconDbContext>()
                        .UseInMemoryDatabase(Guid.NewGuid().ToString())
                        .Options;

        return new ReconDbContext(options, new FakeCurrentTenant(tenantId));
    }

    public static ReconDbContext Create() => Create(Guid.NewGuid());
}