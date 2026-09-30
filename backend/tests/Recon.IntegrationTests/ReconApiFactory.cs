using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Recon.Infrastructure.Persistence;

namespace Recon.IntegrationTests;

// Boots the real API in memory, but replaces SQL Server with an in-memory DB
// so the test needs no Docker/SQL Server running.
public class ReconApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. Remove the real SQL Server registration.
            // Remove ALL EF Core registrations tied to our context, not just one.
            // AddDbContext registers several services; leaving any behind causes
            // "two providers registered" errors.
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<ReconDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                d.ServiceType == typeof(ReconDbContext) ||
                (d.ServiceType.Namespace?.StartsWith("Microsoft.EntityFrameworkCore") ?? false)
            ).ToList();

            foreach (var d in toRemove)
                services.Remove(d);

            // 2. Register the in-memory database instead.
            services.AddDbContext<ReconDbContext>(options =>
                options.UseInMemoryDatabase("IntegrationTestDb")
            );
        });
    }
}