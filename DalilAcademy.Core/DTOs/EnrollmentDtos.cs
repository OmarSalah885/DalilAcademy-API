using DalilAcademy.Core.Enums;

namespace DalilAcademy.Core.DTOs;

public record CreateEnrollmentDto(int StudentId, int CourseId);

public record UpdateEnrollmentDto(int StudentId, int CourseId);

public record EnrollmentResponseDto(
    int Id,
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseTitle,
    DateTime EnrolledAt,
    EnrollmentStatus Status);