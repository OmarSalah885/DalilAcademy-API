using DalilAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DalilAcademy.Infra.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FullName)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(s => s.Email)
               .IsRequired()
               .HasMaxLength(200);

        builder.HasIndex(s => s.Email)
               .IsUnique();   
    }
}