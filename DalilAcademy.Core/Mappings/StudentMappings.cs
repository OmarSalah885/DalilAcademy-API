using DalilAcademy.Core.DTOs;
using DalilAcademy.Core.Entities;

namespace DalilAcademy.Core.Mappings;

public static class StudentMappings
{
    public static StudentResponseDto ToDto(this Student student) => new(
        student.Id,
        student.FullName,
        student.Email);

    public static Student ToEntity(this CreateStudentDto dto) => new()
    {
        FullName = dto.FullName,
        Email = dto.Email
    };
}