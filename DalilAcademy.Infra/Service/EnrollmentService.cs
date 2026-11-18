using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;
using DalilAcademy.Core.Enums;
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
    ICourseRepository courseRepository,
    IPaymentRepository paymentRepository,
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
        var student = await studentRepository.GetByUserIdAsync(userId)
            ?? throw new NotFoundException("Student profile not found.");

        
        var enrollments = await enrollmentRepository.GetPaidByStudentIdAsync(student.Id);

        
        return enrollments
            .Select(e => e.Course.ToDto())   
            .ToList();
    }
    public async Task<EnrollmentResponseDto> CreateAsync(CreateEnrollmentDto dto)
    {
        await createValidator.ValidateAndThrowAsync(dto);

        var course = await courseRepository.GetByIdAsync(dto.CourseId)
            ?? throw new NotFoundException($"Course with id {dto.CourseId} was not found.");

        await EnsureSeatAvailableAsync(course);         

        var enrollment = dto.ToEntity();
        enrollment.Status = course.Price == 0
            ? EnrollmentStatus.Paid
            : EnrollmentStatus.Pending;

        var created = await enrollmentRepository.CreateAsync(enrollment);
        return created.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateEnrollmentDto dto)
    {
        var existing = await enrollmentRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Enrollment with id {id} was not found.");

        if (dto.CourseId != existing.CourseId)
        {
            if (existing.Status == EnrollmentStatus.Paid)
                throw new ConflictException("A paid enrollment cannot be moved to another course.");

            var newCourse = await courseRepository.GetByIdAsync(dto.CourseId)
                ?? throw new NotFoundException($"Course with id {dto.CourseId} was not found.");

            await EnsureSeatAvailableAsync(newCourse);

            var payments = await paymentRepository.GetByEnrollmentIdAsync(id);
            foreach (var p in payments.Where(p => p.Status == PaymentStatus.Created))
            {
                p.Status = PaymentStatus.Failed;
                await paymentRepository.UpdateAsync(p);
            }

            existing.CourseId = dto.CourseId;
            if (newCourse.Price == 0)
                existing.Status = EnrollmentStatus.Paid;
        }

        existing.StudentId = dto.StudentId;

        await enrollmentRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        
        var deleted = await enrollmentRepository.DeleteAsync(id);
        if (!deleted)
            throw new NotFoundException($"Enrollment with id {id} was not found.");
    }
    private async Task EnsureSeatAvailableAsync(Course course)
    {
        if (!course.MaxStudents.HasValue) return;

        var paid = await enrollmentRepository.CountPaidAsync(course.Id);
        if (paid >= course.MaxStudents.Value)
            throw new ConflictException($"Course '{course.Title}' is full ({course.MaxStudents} students).");
    }
}