using Core.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto model)
    {
        await _authService.RegisterAdminAsync(model);
        return Ok(new { Status = "Success", Message = "Administrador criado com sucesso!" });
    }

    [HttpPost("register-user")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
    {
        await _authService.RegisterUserAsync(model);
        return Ok(new { Status = "Success", Message = "Usuário criado com sucesso!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var token = await _authService.LoginAsync(model);
        return Ok(new { token });
    }

    [HttpPost("change-password")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        string username = User.Identity!.Name!;
        await _authService.ChangePasswordAsync(username, model);
        return Ok(new { Status = "Success", Message = "Senha alterada com sucesso!" });
    }

    [HttpPost("reset-password-admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> ResetPasswordAdmin([FromBody] ResetPasswordDto model)
    {
        await _authService.ResetPasswordByAdminAsync(model);
        return Ok(new { Status = "Success", Message = "Senha do usuário redefinida com sucesso pelo administrador!" });
    }
}
