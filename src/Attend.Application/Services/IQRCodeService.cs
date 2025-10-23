namespace Attend.Application.Services;

public interface IQRCodeService
{
    string GenerateQRCodeImage(string text);
    string GenerateQRCodeImageWithText(string qrText, string displayName);
}
