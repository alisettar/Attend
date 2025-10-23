using Attend.Application.Services;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

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

    public string GenerateQRCodeImageWithText(string qrText, string displayName)
    {
        // Generate base QR code
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeBytes = qrCode.GetGraphic(20);
        
        // Load QR as image
        using var qrStream = new MemoryStream(qrCodeBytes);
        using var qrBitmap = new Bitmap(qrStream);
        
        // Calculate new image size (QR + text space)
        int textHeight = 100;
        int finalWidth = qrBitmap.Width;
        int finalHeight = qrBitmap.Height + textHeight;
        
        // Create new image with text space
        using var finalImage = new Bitmap(finalWidth, finalHeight);
        using var graphics = Graphics.FromImage(finalImage);
        
        // White background
        graphics.Clear(Color.White);
        
        // Draw QR code
        graphics.DrawImage(qrBitmap, 0, 0);
        
        // Draw text below QR
        using var font = new Font("Arial", 32, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);
        var textFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        
        var textRect = new RectangleF(0, qrBitmap.Height, finalWidth, textHeight);
        graphics.DrawString(displayName, font, brush, textRect, textFormat);
        
        // Convert to base64
        using var outputStream = new MemoryStream();
        finalImage.Save(outputStream, ImageFormat.Png);
        return Convert.ToBase64String(outputStream.ToArray());
    }
}
