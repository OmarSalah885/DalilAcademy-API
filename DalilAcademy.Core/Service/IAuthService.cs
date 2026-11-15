using DalilAcademy.Core.DTOs.Auth;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto);
}