using DalilAcademy.Core.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
        => Ok(await auth.LoginAsync(dto));

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Register(RegisterRequestDto dto)
    => Ok(await auth.RegisterAsync(dto));
}