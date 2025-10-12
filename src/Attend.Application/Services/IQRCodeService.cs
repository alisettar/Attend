namespace Attend.Application.Services;

public interface IQRCodeService
{
    string GenerateQRCodeImage(string text);
}
