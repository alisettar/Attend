using Attend.Web.Models;

namespace Attend.Web.Services.Interfaces;

public interface IReportService
{
    Task<DashboardStatisticsViewModel?> GetDashboardStatisticsAsync();
}
