namespace Recon.Application.Common.Interfaces;

public interface ICurrentTenant
{
    Guid TenantId { get; }
}