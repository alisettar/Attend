using Attend.Application.Interfaces;
using Attend.Domain.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Attend.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly TenantsConfiguration _tenantsConfig;
    private readonly ILogger<TenantService> _logger;
    private string? _currentTenantId;

    public TenantService(IOptions<TenantsConfiguration> tenantsConfig, ILogger<TenantService> logger)
    {
        _tenantsConfig = tenantsConfig.Value;
        _logger = logger;

        _logger.LogInformation("TenantService initialized. Available tenants: {Tenants}",
            string.Join(", ", _tenantsConfig.Tenants.Keys));
    }

    public string? GetCurrentTenantId() => _currentTenantId;

    public void SetTenantId(string tenantId)
    {
        _logger.LogInformation("SetTenantId called with: {TenantId}", tenantId);
        _logger.LogInformation("Available tenants: {Tenants}", string.Join(", ", _tenantsConfig.Tenants.Keys));

        if (!_tenantsConfig.Tenants.ContainsKey(tenantId))
        {
            _logger.LogError("Tenant '{TenantId}' not found in configuration", tenantId);
            throw new InvalidOperationException($"Tenant '{tenantId}' not found.");
        }

        _currentTenantId = tenantId;
        _logger.LogInformation("Current tenant set to: {TenantId}", tenantId);
    }

    public string GetConnectionString()
    {
        // Return null if tenant not set - will use default connection
        if (string.IsNullOrEmpty(_currentTenantId))
            return null!;

        return _tenantsConfig.Tenants[_currentTenantId].ConnectionString;
    }

    public TenantConfiguration? GetTenantConfiguration()
    {
        if (string.IsNullOrEmpty(_currentTenantId))
            return null;

        return _tenantsConfig.Tenants.TryGetValue(_currentTenantId, out var config)
            ? config
            : null;
    }

    public string? ResolveTenantByUsername(string username)
    {
        foreach (var (tenantId, config) in _tenantsConfig.Tenants)
        {
            if (config.Users.Contains(username, StringComparer.OrdinalIgnoreCase))
                return tenantId;
        }
        return null;
    }

    public string? ResolveTenantByHash(string hash)
    {
        foreach (var (tenantId, config) in _tenantsConfig.Tenants)
        {
            if (config.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Resolved tenant '{TenantId}' from hash '{Hash}'", tenantId, hash);
                return tenantId;
            }
        }

        _logger.LogWarning("No tenant found for hash '{Hash}'", hash);
        return null;
    }
}
