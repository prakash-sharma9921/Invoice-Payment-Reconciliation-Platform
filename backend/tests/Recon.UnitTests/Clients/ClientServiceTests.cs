using Microsoft.EntityFrameworkCore;
using Recon.Application.Clients;
using Recon.Domain.Entities;

namespace Recon.UnitTests.Clients;

public class ClientServiceTests
{
    [Fact]
    public async Task CreateAsync_stamps_current_tenant_and_saves()
    {
        var tenantId = Guid.NewGuid();
        using var db = TestDbContextFactory.Create(tenantId);
        var service = new ClientService(db, new FakeCurrentTenant(tenantId));

        var result = await service.CreateAsync(
            new CreateClientRequest("Client 1", "clien1@gmail.com", null, ClientType.Customer)
        );

        Assert.Equal(tenantId, result.TenantId);
        Assert.Equal("Client 1", result.Name);
        Assert.Equal(1, await db.Clients.CountAsync());
    }
}