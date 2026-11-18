using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DalilAcademy.Infra.Data.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.StudentId, e.CourseId })
       .IsUnique();

        builder.HasOne(e => e.Student)
               .WithMany(s => s.Enrollments)
               .HasForeignKey(e => e.StudentId);

        builder.HasOne(e => e.Course)
               .WithMany(c => c.Enrollments)
               .HasForeignKey(e => e.CourseId);

        builder.Property(e => e.EnrolledAt)
               .HasDefaultValueSql("now()");

        builder.Property(e => e.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(e => e.Status)
               .HasDefaultValue(EnrollmentStatus.Pending);
    }
}