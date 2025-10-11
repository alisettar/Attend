namespace Attend.Domain.Configuration;

public class TenantsConfiguration
{
    public Dictionary<string, TenantConfiguration> Tenants { get; set; } = new();
}
