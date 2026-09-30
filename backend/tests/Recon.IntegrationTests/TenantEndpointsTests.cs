using System.Net;
using System.Net.Http.Json;
using Recon.Application.Tenants;

namespace Recon.IntegrationTests;

public class TenantEndpointsTests : IClassFixture<ReconApiFactory>
{
    private readonly HttpClient _client;

    public TenantEndpointsTests(ReconApiFactory factory) =>
        _client = factory.CreateClient();       // a real HTTP client pointed at the in-memory app

    [Fact]
    public async Task Post_then_get_returns_the_created_tenant()
    {
        // Act 1 — create a tenant over real HTTP
        var createResponse = await _client.PostAsJsonAsync(
            "/api/tenants", new CreateTenantRequest("Test Tenant", "tenant-int")
        );

        // Assert — 201 Created
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<TenantResponse>();
        Assert.NotNull(created);
        Assert.Equal("Test Tenant", created!.Name);

        // Act 2 — list tenants
        var list = await _client.GetFromJsonAsync<List<TenantResponse>>("/api/tenants");

        // Assert — our tenant is in the list
        Assert.NotNull(list);
        Assert.Contains(list!, t => t.Code == "tenant-int");
    }

    [Fact]
    public async Task Post_with_invalid_data_returns_400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/tenants", new CreateTenantRequest("", "Bad code!")
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}