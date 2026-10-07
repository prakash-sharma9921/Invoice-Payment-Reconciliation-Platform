
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Recon.Application.Common.Interfaces;
using System.Reflection;

namespace Recon.Infrastructure.Persistence;

public class ReconDbContextFactory : IDesignTimeDbContextFactory<ReconDbContext>
{
    public ReconDbContext CreateDbContext(string[] args)
    {
        // Build configuration the same sources the app uses, but by hand,
        // because there's no WebApplicationBuilder at design time.
        var configuration = new ConfigurationBuilder()
                                .AddUserSecrets(Assembly.GetExecutingAssembly())
                                .Build();

        var connectionString = configuration.GetConnectionString("ReconDb") ?? throw new InvalidOperationException(
                "Connection string 'ReconDb' not found. Set it with: " +
                "dotnet user-secrets set \"ConnectionStrings:ReconDb\" \"...\" --project src/Recon.Api");

        var options = new DbContextOptionsBuilder<ReconDbContext>().UseSqlServer(connectionString).Options;

        return new ReconDbContext(options, new DesignTimeCurrentTenant());
    }

    private class DesignTimeCurrentTenant : ICurrentTenant
    {
        public Guid TenantId => Guid.Empty;
    }
}