namespace Attend.Web.Models;

internal record ApiPaginationResponse<T>(List<T> Items, long TotalCount);

public record EventStatisticsViewModel(
    int TotalRegistered,
    int TotalCheckedIn,
    int TotalCancelled
);
