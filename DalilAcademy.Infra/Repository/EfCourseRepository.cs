using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Repository;
using DalilAcademy.Infra.Data;

namespace DalilAcademy.Infra.Repository;

public class EfCourseRepository(AppDbContext db) : ICourseRepository
{
    public List<Course> GetAll() => db.Courses.ToList();

    public Course? GetById(int id) => db.Courses.Find(id);

    public Course Create(Course course)
    {
        db.Courses.Add(course);
        db.SaveChanges();
        return course;
    }

    public Course? Update(int id, Course course)
    {
        var existing = db.Courses.Find(id);
        if (existing is null) return null;

        existing.Title = course.Title;
        existing.Description = course.Description;
        existing.Price = course.Price;

        db.SaveChanges();
        return existing;
    }

    public bool Delete(int id)
    {
        var course = db.Courses.Find(id);
        if (course is null) return false;

        db.Courses.Remove(course);
        db.SaveChanges();
        return true;
    }
}