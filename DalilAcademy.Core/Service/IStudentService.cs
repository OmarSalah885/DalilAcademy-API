using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Repository;

namespace DalilAcademy.Core.Service;

public interface IStudentService
{
    Task<List<StudentResponseDto>> GetAllAsync();
    Task<StudentResponseDto> GetByIdAsync(int id);
    Task<List<CourseResponseDto>> GetCoursesByStudentIdAsync(int studentId);

    Task<List<CourseResponseDto>> GetCoursesByUserIdAsync(int userId);
    Task<PagedResult<StudentResponseDto>> GetPagedAsync(int page, int pageSize);
    Task<StudentResponseDto> CreateAsync(CreateStudentDto dto);
    Task UpdateAsync(int id, UpdateStudentDto dto);
    Task DeleteAsync(int id);
}