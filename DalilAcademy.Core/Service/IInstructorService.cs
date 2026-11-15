using DalilAcademy.Core.DTOs;

namespace DalilAcademy.Core.Service;

public interface IInstructorService
{
    Task<List<InstructorResponseDto>> GetAllAsync();
    Task<InstructorResponseDto> GetByIdAsync(int id);
    Task<InstructorResponseDto> CreateAsync(CreateInstructorDto dto);
    Task UpdateAsync(int id, UpdateInstructorDto dto);
    Task DeleteAsync(int id);
}