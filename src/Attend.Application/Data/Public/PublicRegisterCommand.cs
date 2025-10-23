using Attend.Application.Interfaces;
using Attend.Application.Repositories;
using Attend.Application.Services;
using Attend.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Attend.Application.Data.Public;

public sealed record PublicRegisterRequest(string Name, string Phone, string? RecaptchaToken = null);

public sealed record PublicRegisterResult(Guid UserId, string QRCodeImage, string UserName);

public sealed record PublicRegisterCommand(string TenantHash, PublicRegisterRequest Request)
    : IRequest<PublicRegisterResult>;

public sealed class PublicRegisterCommandHandler(
    IServiceProvider serviceProvider,
    IQRCodeService qrCodeService,
    ITenantService tenantService,
    IReCaptchaService reCaptchaService)
    : IRequestHandler<PublicRegisterCommand, PublicRegisterResult>
{
    public async Task<PublicRegisterResult> Handle(
        PublicRegisterCommand request,
        CancellationToken cancellationToken)
    {
        // Verify reCAPTCHA token
        if (!string.IsNullOrEmpty(request.Request.RecaptchaToken))
        {
            var isValidToken = await reCaptchaService.VerifyTokenAsync(
                request.Request.RecaptchaToken,
                cancellationToken);

            if (!isValidToken)
                throw new ValidationException("Security verification failed. Please try again.");
        }

        // Resolve tenant by hash
        var tenantId = tenantService.ResolveTenantByHash(request.TenantHash);
        if (tenantId == null)
            throw new ValidationException("Invalid registration link.");

        // Set tenant context
        tenantService.SetTenantId(tenantId);

        // Create a new scope and get repository with proper tenant context
        using var scope = serviceProvider.CreateScope();
        var scopedTenantService = scope.ServiceProvider.GetRequiredService<ITenantService>();
        scopedTenantService.SetTenantId(tenantId);

        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        // Check for duplicate phone in this tenant
        var phoneExists = await repository.ExistsByPhoneAsync(
            request.Request.Phone,
            null,
            cancellationToken);

        if (phoneExists)
            throw new ValidationException("This phone number is already registered.");

        // Create user
        var user = User.Create(
            name: request.Request.Name,
            phone: request.Request.Phone);

        // Generate QR code
        user.QRCodeImage = qrCodeService.GenerateQRCodeImage(user.QRCode);

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
    private static readonly Regex TurkishPhoneRegex = new(
        @"^(\+90|0)?5\d{9}$",
        RegexOptions.Compiled);

    public PublicRegisterCommandValidator()
    {
        RuleFor(x => x.TenantHash)
            .NotEmpty()
            .WithMessage("Invalid registration link.");

        RuleFor(x => x.Request.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Request.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .Matches(TurkishPhoneRegex)
            .WithMessage("Please enter a valid Turkish phone number (e.g., 05XX XXX XX XX)");
    }
}
