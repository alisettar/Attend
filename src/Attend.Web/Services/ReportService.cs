using Attend.Web.Models;
using Attend.Web.Services.Interfaces;
using System.Text.Json;

namespace Attend.Web.Services;

public class ReportService(IHttpClientFactory httpClientFactory, ILogger<ReportService> logger) : IReportService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AttendApi");
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<DashboardStatisticsViewModel?> GetDashboardStatisticsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/reports/statistics");
            if (!response.IsSuccessStatusCode) return null;
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<DashboardStatisticsViewModel>(json, _jsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting dashboard statistics");
            return null;
        }
    }
}
