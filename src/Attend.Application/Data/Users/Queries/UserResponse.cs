using Attend.Domain.Entities;

namespace Attend.Application.Data.Users.Queries;

public record UserResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string QRCode,
    string? QRCodeImage,
    DateTime CreatedAt,
    int AttendanceCount)
{
    public static UserResponse FromDomain(User user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Phone,
            user.QRCode,
            user.QRCodeImage,
            user.CreatedAt,
            user.Attendances?.Count ?? 0);
    }

    public static List<UserResponse> FromDomainList(List<User> users)
    {
        return users.Select(FromDomain).ToList();
    }
}
