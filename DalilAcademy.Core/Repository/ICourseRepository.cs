using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface ICourseRepository
{
    List<Course> GetAll();
    Course? GetById(int id);
    Course Create(Course course);
    Course? Update(int id, Course course);
    bool Delete(int id);                     
}