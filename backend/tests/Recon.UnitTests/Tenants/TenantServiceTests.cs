using Microsoft.EntityFrameworkCore;
using Recon.Application.Tenants;

namespace Recon.UnitTests.Tenants;

public class TenantServiceTests
{
    [Fact] // [Fact] marks a single test method
    public async Task CreateAsync_saves_a_tenant_and_returns_it()
    {
        // Arrange — set up the fake DB and the service under test
        using var db = TestDbContextFactory.Create();
        var service = new TenantService(db);
        var request = new CreateTenantRequest("Test Tenant", "testtenant");

        // Act — run the thing we're testing
        var result = await service.CreateAsync(request);

        // Assert — check the outcome is what we expect
        Assert.NotEqual(Guid.Empty, result.Id);     // a GUID was generated
        Assert.Equal("Test Tenant", result.Name);       // Created tenant name matches
        Assert.True(result.IsActive);   // new tenants are active
        Assert.Equal(1, await db.Tenants.CountAsync());     // it's actually in the DB
    }

    [Fact]
    public async Task GetAllAsync_returns_tenants_ordered_by_name()
    {
        // Arrange — pre-load two tenants, deliberately out of order
        using var db = TestDbContextFactory.Create();
        var service = new TenantService(db);
        await service.CreateAsync(new CreateTenantRequest("Test name 1", "testname1"));
        await service.CreateAsync(new CreateTenantRequest("Test name 2", "testname2"));

        // Act
        var result = await service.GetAllAsync();

        // Assert — should come back alphabetically
        Assert.Equal(2, result.Count);
        Assert.Equal("Test name 1", result[0].Name);
        Assert.Equal("Test name 2", result[1].Name);
    }
}