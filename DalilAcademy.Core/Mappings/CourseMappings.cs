using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Mappings;

public static class CourseMappings
{
    public static CourseResponseDto ToDto(this Course course) => new(
        course.Id,
        course.Title,
        course.Description,
        course.Price,
        course.Level,
        course.Instructor?.FullName,
        course.ImageUrl,
        course.MaxStudents);

    public static Course ToEntity(this CreateCourseDto dto) => new()
    {
        Title = dto.Title,
        Description = dto.Description,
        Price = dto.Price,
        Level = dto.Level,
        InstructorId = dto.InstructorId,
        ImageUrl= dto.ImageUrl,
        MaxStudents = dto.MaxStudents
    };
}