using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Attend.Domain.Entities;

namespace Attend.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .HasMaxLength(200);

        builder.HasIndex(u => u.Email);

        builder.Property(u => u.Phone)
            .HasMaxLength(50);

        builder.Property(u => u.QRCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.QRCode)
            .IsUnique();

        builder.Property(u => u.QRCodeImage);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.HasMany(u => u.Attendances)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
