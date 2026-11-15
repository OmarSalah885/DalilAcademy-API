using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Mappings;

public static class EnrollmentMappings
{
    public static EnrollmentResponseDto ToDto(this Enrollment enrollment) => new(
        enrollment.Id,
        enrollment.StudentId,
        enrollment.Student?.FullName ?? string.Empty,
        enrollment.CourseId,
        enrollment.Course?.Title ?? string.Empty,
        enrollment.EnrolledAt);

    public static Enrollment ToEntity(this CreateEnrollmentDto dto) => new()
    {
        StudentId = dto.StudentId,
        CourseId = dto.CourseId
    };
}