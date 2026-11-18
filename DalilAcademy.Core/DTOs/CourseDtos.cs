namespace DalilAcademy.Core.DTOs;

public record CreateCourseDto(string Title, string? Description, decimal Price, string Level,
    int InstructorId, string? ImageUrl, int? MaxStudents = null);

public record UpdateCourseDto(string Title, string? Description, decimal Price, string Level,
    string? ImageUrl, int? MaxStudents = null);

public record CourseResponseDto(int Id, string Title, string? Description, decimal Price,
    string Level, string? InstructorName, string? ImageUrl, int? MaxStudents = null);

public record EnrollmentSummaryDto(
    int StudentId,
    string StudentFullName,
    string StudentEmail,
    DateTime EnrolledAt
);

public record CourseWithEnrollmentsDto(
    int Id,
    string Title,
    string InstructorName,
    List<EnrollmentSummaryDto> Enrollments
);