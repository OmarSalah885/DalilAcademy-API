using DalilAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Student> Students => Set<Student>();   // ← added

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {
            // Seed Students first
            if (!context.Set<Student>().Any())
            {
                context.Set<Student>().AddRange(
                    new Student { FullName = "Omar Salah", Email = "omar@example.com" },
                    new Student { FullName = "Sara Ali", Email = "sara@example.com" },
                    new Student { FullName = "Ahmad Hassan", Email = "ahmad@example.com" }
                );
            }

            // Seed Courses
            if (!context.Set<Course>().Any())
            {
                context.Set<Course>().AddRange(
                    new Course
                    {
                        Title = "C# Fundamentals",
                        Description = "Learn the basics of C# programming",
                        Price = 49.99m,
                        Level = "Beginner"
                    },
                    new Course
                    {
                        Title = "ASP.NET Core Web API",
                        Description = "Build modern APIs with ASP.NET Core",
                        Price = 79.99m,
                        Level = "Intermediate"
                    },
                    new Course
                    {
                        Title = "Entity Framework Core",
                        Description = "Master EF Core and Code First",
                        Price = 69.99m,
                        Level = "Intermediate"
                    },
                    new Course
                    {
                        Title = "PostgreSQL for Developers",
                        Description = "Working with PostgreSQL in real projects",
                        Price = 59.99m,
                        Level = "Beginner"
                    }
                );
            }

            context.SaveChanges();
        });
    }
}