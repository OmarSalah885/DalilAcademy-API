using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Mappings;
using DalilAcademy.Core.Repository;
using DalilAcademy.Core.Service;
using DalilAcademy.Infra.Repository;
using FluentValidation;

namespace DalilAcademy.Infra.Service;

public class EnrollmentService(
    IEnrollmentRepository enrollmentRepository,
    IStudentRepository studentRepository,
    IValidator<CreateEnrollmentDto> createValidator) : IEnrollmentService
{
    public async Task<List<EnrollmentResponseDto>> GetAllAsync()
    {
        var enrollments = await enrollmentRepository.GetAllAsync();
        return enrollments.Select(e => e.ToDto()).ToList();
    }

    public async Task<EnrollmentResponseDto> GetByIdAsync(int id)
    {
        var enrollment = await enrollmentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Enrollment with id {id} was not found.");

        return enrollment.ToDto();
    }

    public async Task<List<CourseResponseDto>> GetMyCoursesAsync(int userId)
    {
        var student = await studentRepository.GetByUserIdAsync(userId);

        if (student is null)
            throw new NotFoundException("Student profile not found.");

        var courses = await studentRepository.GetCoursesByStudentIdAsync(student.Id);

        
        return courses;
    }

    public async Task<EnrollmentResponseDto> CreateAsync(CreateEnrollmentDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);



        var enrollment = dto.ToEntity();
        var created = await enrollmentRepository.CreateAsync(enrollment);

        return created.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await enrollmentRepository.DeleteAsync(id);
        if (!deleted)
            throw new NotFoundException($"Enrollment with id {id} was not found.");
    }
}