namespace Attend.Application.Services;

public interface IReCaptchaService
{
    Task<bool> VerifyTokenAsync(string token, CancellationToken cancellationToken = default);
}
