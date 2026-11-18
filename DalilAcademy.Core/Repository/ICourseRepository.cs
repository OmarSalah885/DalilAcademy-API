using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Repository;

public interface ICourseRepository
{
    Task<List<CourseResponseDto>> GetAllAsync();
    Task<PagedResult<CourseResponseDto>> GetPagedAsync(int page, int pageSize,
        decimal? minPrice = null, decimal? maxPrice = null, string? search = null);
    Task<int> GetCountAsync(decimal? minPrice = null, decimal? maxPrice = null, string? search = null);
    Task<Course?> GetByIdAsync(int id);
    Task<Course> CreateAsync(Course course);
    Task UpdateAsync(Course course);          
    Task<bool> DeleteAsync(int id);
    Task<List<CourseWithEnrollmentsDto>> GetAllWithEnrollmentsAsync();
    Task<List<StudentResponseDto>> GetPaidStudentsAsync(int courseId);
}