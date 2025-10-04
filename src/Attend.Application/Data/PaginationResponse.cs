namespace Attend.Application.Data;

public record PaginationResponse<T>(List<T> Items, long TotalCount);
