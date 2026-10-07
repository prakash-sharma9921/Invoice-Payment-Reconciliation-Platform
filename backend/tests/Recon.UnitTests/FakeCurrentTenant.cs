using Recon.Application.Common.Interfaces;

namespace Recon.UnitTests;

public class FakeCurrentTenant : ICurrentTenant
{
    public Guid TenantId { get; }
    public FakeCurrentTenant(Guid tenantId) => TenantId = tenantId;
}