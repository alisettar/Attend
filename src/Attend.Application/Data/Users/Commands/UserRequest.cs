namespace Attend.Application.Data.Users.Commands;

public record UserRequest(
    Guid? Id,
    string Name,
    string? Email,
    string? Phone);
