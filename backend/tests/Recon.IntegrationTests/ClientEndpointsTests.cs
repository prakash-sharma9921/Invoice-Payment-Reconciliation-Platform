using System.Net;
using System.Net.Http.Json;
using Recon.Application.Clients;
using Recon.Domain.Entities;

namespace Recon.IntegrationTests;

public class ClientEndpointsTests : IClassFixture<ReconApiFactory>
{
    private readonly HttpClient _client;
    public ClientEndpointsTests(ReconApiFactory factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Create_then_list_with_tenant_header_works()
    {
        _client.DefaultRequestHeaders.Add("X-Tenant-Id", Guid.NewGuid().ToString());

        var create = await _client.PostAsJsonAsync("/api/clients",
            new CreateClientRequest("Prakash", "prakash@gmail.com", null, ClientType.Customer));

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var list = await _client.GetFromJsonAsync<List<ClientResponse>>("/api/Clients");

        Assert.NotNull(list);
        Assert.Contains(list!, c => c.Name == "Prakash");
    }
}