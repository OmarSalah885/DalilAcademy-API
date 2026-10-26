using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;

namespace DalilAcademy.Infra.Service;

public class CourseService(ICourseRepository repo) : ICourseService
{
    public List<Course> GetAll() => repo.GetAll();

    public Course? GetById(int id) => repo.GetById(id);

    public Course Create(Course course)
    {
        if (course.Price < 0)
            throw new ArgumentException("Price cannot be negative.");

        // Unique title rule
        if (repo.GetAll().Any(c =>
            c.Title.Equals(course.Title, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Course title must be unique.");
        }

        return repo.Create(course);
    }

    public Course? Update(int id, Course course)
    {
        if (course.Price < 0)
            throw new ArgumentException("Price cannot be negative.");

        // Unique title rule (ignore the current course itself)
        if (repo.GetAll().Any(c =>
            c.Id != id &&
            c.Title.Equals(course.Title, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("Course title must be unique.");
        }

        return repo.Update(id, course);
    }

    public bool Delete(int id) => repo.Delete(id);
}