using DalilAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DalilAcademy.Infra.Data.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(e => e.Id);
            builder.HasOne(e=>e.Student).WithMany(e => e.Enrollments).HasForeignKey(e => e.StudentId);
            builder.HasOne(e => e.Course).WithMany(e => e.Enrollments).HasForeignKey(e => e.CourseId);
            builder.Property(e => e.EnrolledAt).HasDefaultValueSql("now()");

        }
    }
}
