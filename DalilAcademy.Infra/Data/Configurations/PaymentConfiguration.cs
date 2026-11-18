using DalilAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DalilAcademy.Infra.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasOne(p => p.Enrollment)
               .WithMany(e => e.Payments)
               .HasForeignKey(p => p.EnrollmentId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Property(p => p.Amount)
               .HasPrecision(10, 2)
               .IsRequired();

        builder.Property(p => p.Currency)
               .HasMaxLength(3)
               .IsRequired();

        builder.Property(p => p.PayPalOrderId)
               .HasMaxLength(100)
               .IsRequired();

        
        builder.HasIndex(p => p.PayPalOrderId)
               .IsUnique();

        
        builder.Property(p => p.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(p => p.CreatedAt)
               .IsRequired();

        builder.Property(p => p.PaidAt)
               .IsRequired(false);
    }
}