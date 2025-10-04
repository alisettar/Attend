namespace Attend.Web.Models;

internal record ApiPaginationResponse<T>(List<T> Items, long TotalCount);
