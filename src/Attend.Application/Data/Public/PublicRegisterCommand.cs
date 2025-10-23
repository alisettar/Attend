using MediatR;
using Attend.Application.Repositories;
using Attend.Application.Services;
using Attend.Application.Interfaces;
using Attend.Domain.Entities;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Attend.Application.Data.Public;

public sealed record PublicRegisterRequest(string Name, string Phone);

public sealed record PublicRegisterResult(Guid UserId, string QRCodeImage, string UserName);

public sealed record PublicRegisterCommand(string TenantHash, PublicRegisterRequest Request) 
    : IRequest<PublicRegisterResult>;

public sealed class PublicRegisterCommandHandler(
    IUserRepository repository,
    IQRCodeService qrCodeService,
    ITenantService tenantService) 
    : IRequestHandler<PublicRegisterCommand, PublicRegisterResult>
{
    public async Task<PublicRegisterResult> Handle(
        PublicRegisterCommand request, 
        CancellationToken cancellationToken)
    {
        // Resolve tenant by hash
        var tenantId = tenantService.ResolveTenantByHash(request.TenantHash);
        if (tenantId == null)
            throw new ValidationException("Invalid registration link.");

        // Set tenant context
        tenantService.SetTenantId(tenantId);

        // Check for duplicate phone in this tenant
        var phoneExists = await repository.ExistsByPhoneAsync(
            request.Request.Phone, 
            null, 
            cancellationToken);
        
        if (phoneExists)
            throw new ValidationException("Bu telefon numarası zaten kayıtlı.");

        // Create user
        var user = User.Create(
            name: request.Request.Name,
            email: null,
            phone: request.Request.Phone);

        // Generate QR code (without text for now)
        user.QRCodeImage = qrCodeService.GenerateQRCodeImage(user.QRCode);
        // Alternative with text: qrCodeService.GenerateQRCodeImageWithText(user.QRCode, user.Name);

        // Save to database
        await repository.AddAsync(user, cancellationToken);

        return new PublicRegisterResult(
            UserId: user.Id,
            QRCodeImage: user.QRCodeImage ?? string.Empty,
            UserName: user.Name);
    }
}

public sealed class PublicRegisterCommandValidator : AbstractValidator<PublicRegisterCommand>
{
    // Turkish phone regex: +90 5XX XXX XX XX or 05XX XXX XX XX
    private static readonly Regex TurkishPhoneRegex = new(
        @"^(\+90|0)?5\d{9}$",
        RegexOptions.Compiled);

    public PublicRegisterCommandValidator()
    {
        RuleFor(x => x.TenantHash)
            .NotEmpty()
            .WithMessage("Geçersiz kayıt linki.");

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Ad Soyad boş olamaz.")
            .MaximumLength(200)
            .WithMessage("Ad Soyad 200 karakterden uzun olamaz.");

        RuleFor(x => x.Request.Phone)
            .NotEmpty()
            .WithMessage("Telefon numarası boş olamaz.")
            .Matches(TurkishPhoneRegex)
            .WithMessage("Geçerli bir Türkiye telefon numarası giriniz (örn: 05XX XXX XX XX)");
    }
}
