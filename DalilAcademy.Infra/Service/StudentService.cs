using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using DalilAcademy.Infra.Repository;
using FluentValidation;

namespace DalilAcademy.Infra.Service;

public class StudentService(
    IStudentRepository studentRepository,
    IValidator<CreateStudentDto> createValidator) : IStudentService
{
    public async Task<List<StudentResponseDto>> GetAllAsync()
    {
        var students = await studentRepository.GetAllAsync();
        return students.Select(s => s.ToDto()).ToList();
    }


    public async Task<StudentResponseDto> GetByIdAsync(int id)
    {
        var student = await studentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student with id {id} was not found.");

        return student.ToDto();
    }
    // Search by Student ID
    public async Task<List<CourseResponseDto>> GetCoursesByStudentIdAsync(int studentId)
    {
        return await studentRepository.GetCoursesByStudentIdAsync(studentId);
    }

    // Search by User ID
    public async Task<List<CourseResponseDto>> GetCoursesByUserIdAsync(int userId)
    {
        return await studentRepository.GetCoursesByUserIdAsync(userId);
    }
    public async Task<PagedResult<StudentResponseDto>> GetPagedAsync(int page, int pageSize)
    {
        return await studentRepository.GetPagedAsync(page, pageSize);
    }

    public async Task<StudentResponseDto> CreateAsync(CreateStudentDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);

        var student = dto.ToEntity();
        var created = await studentRepository.CreateAsync(student);

        return created.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateStudentDto dto)
    {
        var existing = await studentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Student with id {id} was not found.");

        existing.FullName = dto.FullName;
        existing.Email = dto.Email;

        await studentRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await studentRepository.DeleteAsync(id);
        if (!deleted)
            throw new NotFoundException($"Student with id {id} was not found.");
    }
}