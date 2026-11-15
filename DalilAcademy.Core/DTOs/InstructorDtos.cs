namespace DalilAcademy.Core.DTOs;

public record CreateInstructorDto(string FullName, string Email);

public record UpdateInstructorDto(string FullName, string Email);

public record InstructorResponseDto(int Id, string FullName, string Email);