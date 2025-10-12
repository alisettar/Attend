using Attend.Domain.BaseClasses;

namespace Attend.Domain.Entities;

public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string QRCode { get; private set; } = string.Empty;
    public string? QRCodeImage { get; set; } // Base64 PNG
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<Attendance> Attendances { get; set; } = new();

    public static User Create(string name, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        var user = new User
        {
            Name = name.Trim(),
            Email = email?.Trim(),
            Phone = phone?.Trim()
        };

        user.QRCode = GenerateQRCode(user.Id);
        return user;
    }

    public static User Update(User currentUser, string name, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        currentUser.Name = name.Trim();
        currentUser.Email = email?.Trim();
        currentUser.Phone = phone?.Trim();

        return currentUser;
    }

    private static string GenerateQRCode(Guid userId)
    {
        return $"USER-{userId:N}";
    }
}
