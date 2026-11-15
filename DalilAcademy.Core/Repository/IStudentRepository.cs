using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface IStudentRepository
{
    Task<List<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<List<CourseResponseDto>> GetCoursesByStudentIdAsync(int studentId);

    Task<List<CourseResponseDto>> GetCoursesByUserIdAsync(int userId);

    Task<PagedResult<StudentResponseDto>> GetPagedAsync(int page, int pageSize);
    Task<Student?> GetByUserIdAsync(int userId);

    Task<Student> CreateAsync(Student student);

    Task UpdateAsync(Student student);
    Task<bool> DeleteAsync(int id);
}