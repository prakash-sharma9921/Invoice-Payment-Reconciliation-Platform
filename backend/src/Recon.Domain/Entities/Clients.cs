namespace Recon.Domain.Entities;

public class Client
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public ClientType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public enum ClientType
{
    Customer = 1,
    Vendor = 2,
    Both = 3
}