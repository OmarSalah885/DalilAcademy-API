using DalilAcademy.Core.DTOs.Auth;
using DalilAcademy.Core.Exceptions;
using DalilAcademy.Core.Repository;

public class AuthService(
    IUserRepository users,
    IPasswordHasher hasher,
    ITokenService tokens,
    IStudentRepository students) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await users.GetByEmailAsync(dto.Email);

        if (user is null || !hasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var (token, expiresAt) = tokens.CreateToken(user);
        return new LoginResponseDto(token, expiresAt);
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto)
    {
        var existing = await users.GetByEmailAsync(dto.Email);
        if (existing is not null)
            throw new ConflictException("Email is already registered.");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = hasher.Hash(dto.Password),
            Role = Roles.Student
        };

        await users.AddAsync(user);

        var student = new Student
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserId = user.Id
        };

        await students.CreateAsync(student);

        var (token, expiresAt) = tokens.CreateToken(user);
        return new LoginResponseDto(token, expiresAt);
    }
}