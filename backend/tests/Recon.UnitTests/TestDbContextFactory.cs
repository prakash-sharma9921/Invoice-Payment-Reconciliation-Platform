using Microsoft.EntityFrameworkCore;
using Recon.Infrastructure.Persistence;

namespace Recon.UnitTests;

public static class TestDbContextFactory
{
    public static ReconDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ReconDbContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;

        return new ReconDbContext(options);
    }
}