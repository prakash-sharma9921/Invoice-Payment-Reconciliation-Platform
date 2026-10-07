using Microsoft.EntityFrameworkCore;
using Recon.Domain.Entities;
using Recon.Infrastructure.Persistence;

namespace Recon.UnitTests.Clients;

public class ClientIsolationTests
{
    [Fact]
    public async Task Tenant_sees_only_its_own_clients()
    {
        var tenantA = Guid.NewGuid();
        var tenantB = Guid.NewGuid();
        var dbName = Guid.NewGuid().ToString();

        var options = new DbContextOptionsBuilder<ReconDbContext>()
            .UseInMemoryDatabase(dbName).Options;

        using (var db = new ReconDbContext(options, new FakeCurrentTenant(tenantA)))
        {
            db.Clients.Add(new Client
            {
                Id = Guid.NewGuid(),
                TenantId = tenantA,
                Name = "Client A",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }

        using (var db = new ReconDbContext(options, new FakeCurrentTenant(tenantB)))
        {
            db.Clients.Add(new Client
            {
                Id = Guid.NewGuid(),
                TenantId = tenantB,
                Name = "Client B",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }

        // Tenant A queries — the global filter should hide B's client automatically.
        using (var db = new ReconDbContext(options, new FakeCurrentTenant(tenantA)))
        {
            var clients = await db.Clients.ToListAsync();
            Assert.Single(clients);
            Assert.Equal("Client A", clients[0].Name);
        }

    }
}