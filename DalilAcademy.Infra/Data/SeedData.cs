using DalilAcademy.Core.Entities;

namespace DalilAcademy.Infra.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        // Seed Admin User
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Email = "admin@dalil.jo",
                Role = Roles.Admin,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123")
            });
            db.SaveChanges();
        }

        // Only seed the rest if the database is empty
        if (db.Instructors.Any()) return;

        var ahmad = new Instructor
        {
            FullName = "Ahmed Al-Rashidi",
            Email = "ahmed@dalil.com"
        };

        var sara = new Instructor
        {
            FullName = "Sara Hassan",
            Email = "sara@dalil.com"
        };

        db.Instructors.AddRange(ahmad, sara);
        db.SaveChanges();

        var course1 = new Course
        {
            Title = "ASP.NET Core Web API",
            Description = "Build production-ready APIs",
            Price = 49.99m,
            Level = "Intermediate",
            InstructorId = ahmad.Id
        };

        var course2 = new Course
        {
            Title = "Entity Framework Core",
            Description = "Master EF Core",
            Price = 39.99m,
            Level = "Beginner",
            InstructorId = ahmad.Id
        };

        var course3 = new Course
        {
            Title = "Clean Architecture",
            Description = "Build maintainable applications",
            Price = 59.99m,
            Level = "Advanced",
            InstructorId = sara.Id
        };

        db.Courses.AddRange(course1, course2, course3);
        db.SaveChanges();

        var omar = new Student
        {
            FullName = "Omar Salah",
            Email = "omar@example.com"
        };

        var layla = new Student
        {
            FullName = "Layla Mahmoud",
            Email = "layla@example.com"
        };

        db.Students.AddRange(omar, layla);
        db.SaveChanges();

        db.Enrollments.AddRange(
            new Enrollment { StudentId = omar.Id, CourseId = course1.Id },
            new Enrollment { StudentId = omar.Id, CourseId = course2.Id },
            new Enrollment { StudentId = layla.Id, CourseId = course1.Id },
            new Enrollment { StudentId = layla.Id, CourseId = course3.Id }
        );

        db.SaveChanges();
    }
}