using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Repository;

namespace DalilAcademy.Infra.Repository;

public class CourseRepository : ICourseRepository
{
    private readonly List<Course> _courses = new();

    public List<Course> GetAll() => _courses;

    public Course? GetById(int id) =>
        _courses.FirstOrDefault(c => c.Id == id);

    public Course Create(Course course)
    {
        course.Id = _courses.Count == 0 ? 1 : _courses.Max(c => c.Id) + 1;
        _courses.Add(course);
        return course;
    }

    public Course? Update(int id, Course course)
    {
        var existing = GetById(id);
        if (existing is null)
            return null;

        existing.Title = course.Title;
        existing.Description = course.Description;
        existing.Price = course.Price;

        return existing;
    }

    public bool Delete(int id)
    {
        var course = GetById(id);
        if (course is null)
            return false;

        _courses.Remove(course);
        return true;
    }
}