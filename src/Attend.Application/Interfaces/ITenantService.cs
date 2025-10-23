using Attend.Domain.Configuration;

namespace Attend.Application.Interfaces;

public interface ITenantService
{
    string? GetCurrentTenantId();
    void SetTenantId(string tenantId);
    string GetConnectionString();
    TenantConfiguration? GetTenantConfiguration();
    string? ResolveTenantByUsername(string username);
    string? ResolveTenantByHash(string hash);
}
