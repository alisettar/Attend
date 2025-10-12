using Attend.Application.Services;
using QRCoder;

namespace Attend.Infrastructure.Services;

public class QRCodeService : IQRCodeService
{
    public string GenerateQRCodeImage(string text)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(qrCodeBytes);
    }
}
