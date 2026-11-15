using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Mappings;

public static class InstructorMappings
{
    public static InstructorResponseDto ToDto(this Instructor instructor) => new(
        instructor.Id,
        instructor.FullName,
        instructor.Email);

    public static Instructor ToEntity(this CreateInstructorDto dto) => new()
    {
        FullName = dto.FullName,
        Email = dto.Email
    };
}