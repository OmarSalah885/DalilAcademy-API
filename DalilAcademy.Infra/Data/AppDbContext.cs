using DalilAcademy.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DalilAcademy.Infra.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((context, _) =>
        {
            if (context.Set<Course>().Any()) return; // only seed if empty

            context.Set<Course>().AddRange(
                new Course
                {
                    Title = "C# Fundamentals",
                    Description = "Learn the basics of C# programming",
                    Price = 49.99m
                },
                new Course
                {
                    Title = "ASP.NET Core Web API",
                    Description = "Build modern APIs with ASP.NET Core",
                    Price = 79.99m
                },
                new Course
                {
                    Title = "Entity Framework Core",
                    Description = "Master EF Core and Code First",
                    Price = 69.99m
                },
                new Course
                {
                    Title = "PostgreSQL for Developers",
                    Description = "Working with PostgreSQL in real projects",
                    Price = 59.99m
                }
            );

            context.SaveChanges();
        });
    }
}