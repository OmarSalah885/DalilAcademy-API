using DalilAcademy.Core.DTOs;

namespace DalilAcademy.Core.Service;

public interface IEnrollmentService
{
    Task<List<EnrollmentResponseDto>> GetAllAsync();
    Task<EnrollmentResponseDto> GetByIdAsync(int id);
    Task<EnrollmentResponseDto> CreateAsync(CreateEnrollmentDto dto);
    Task DeleteAsync(int id);
}