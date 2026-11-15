namespace DalilAcademy.Core.DTOs;

public record CreateEnrollmentDto(int StudentId, int CourseId);

public record EnrollmentResponseDto(
    int Id,
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseTitle,
    DateTime EnrolledAt);