using Attend.Domain.Entities;

namespace Attend.Application.Data.Users.Queries;

public record UserResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string QRCode,
    DateTime CreatedAt)
{
    public static UserResponse FromDomain(User user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Phone,
            user.QRCode,
            user.CreatedAt);
    }

    public static List<UserResponse> FromDomainList(List<User> users)
    {
        return users.Select(FromDomain).ToList();
    }
}
