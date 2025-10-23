namespace Attend.Web.Models;

public class RegisterFormModel
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool AcceptedTerms { get; set; }
    public string? RecaptchaToken { get; set; }
}

public class PublicRegisterResultDto
{
    public Guid UserId { get; set; }
    public string QRCodeImage { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? QRCodeImage { get; set; }
}
