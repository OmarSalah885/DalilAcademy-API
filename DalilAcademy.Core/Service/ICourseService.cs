using DalilAcademy.Core.DTOs;

namespace DalilAcademy.Core.Service;

public interface ICourseService
{
    Task<List<CourseResponseDto>> GetAllAsync();
    Task<PagedResult<CourseResponseDto>> GetPagedAsync(int page, int pageSize,
        decimal? minPrice = null, decimal? maxPrice = null, string? search = null);
    Task<int> GetCountAsync(decimal? minPrice = null, decimal? maxPrice = null, string? search = null);
    Task<CourseResponseDto> GetByIdAsync(int id);
    Task<CourseResponseDto> CreateAsync(CreateCourseDto dto);
    Task UpdateAsync(int id, UpdateCourseDto dto);
    Task DeleteAsync(int id);
    Task<List<CourseWithEnrollmentsDto>> GetAllWithEnrollmentsAsync();
    Task<List<StudentResponseDto>> GetPaidStudentsAsync(int courseId);
}