namespace DalilAcademy.Core.DTOs.Auth
{
    public record RegisterRequestDto(
    string Email,
    string Password,
    string FullName          
);
}
