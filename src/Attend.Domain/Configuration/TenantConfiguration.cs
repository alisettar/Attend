namespace Attend.Domain.Configuration;

public class TenantConfiguration
{
    public string Name { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public string? WhatsAppGroupUrl { get; set; }
    public List<string> Users { get; set; } = new();
}
