using Recon.Application.Common.Interfaces;

namespace Recon.Api.Tenancy;

public class HeaderCurrentTenant : ICurrentTenant
{

    // TEMPORARY dev mechanism: reads the tenant id from the X-Tenant-Id header.
    // Will be replaced by reading from the login token once auth is added.
    private readonly IHttpContextAccessor _accessor;

    public HeaderCurrentTenant(IHttpContextAccessor accessor) => _accessor = accessor;

    public Guid TenantId
    {
        get
        {
            var header = _accessor.HttpContext?.Request.Headers["X-Tenant-Id"].ToString();

            if (Guid.TryParse(header, out var id))
                return id;
            else
                throw new InvalidOperationException("Invalid or missing X-Tenant-Id header");
        }
    }
}