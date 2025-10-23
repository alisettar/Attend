using Attend.Domain.BaseClasses;
using Attend.Domain.Extensions;

namespace Attend.Domain.Entities;

public class User : Entity
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty; // Required field
    public string QRCode { get; private set; } = string.Empty;
    public string? QRCodeImage { get; set; } // Base64 PNG
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<Attendance> Attendances { get; set; } = new();

    public static User Create(string name, string phone, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required", nameof(phone));

        var user = new User
        {
            Name = name.NormalizeFullName(),
            Phone = phone.Trim(),
            Email = email?.Trim()
        };

        user.QRCode = GenerateQRCode(user.Id);
        return user;
    }

    public static User Update(User currentUser, string name, string phone, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required", nameof(phone));

        currentUser.Name = name.NormalizeFullName();
        currentUser.Phone = phone.Trim();
        currentUser.Email = email?.Trim();

        return currentUser;
    }

    private static string GenerateQRCode(Guid userId)
    {
        return $"USER-{userId:N}";
    }
}
