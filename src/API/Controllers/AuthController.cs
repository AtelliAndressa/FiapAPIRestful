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

    /// <summary>
    /// Permite que um Administrador já autenticado crie uma nova conta de administrador no sistema. 
    /// É protegido pela política AdminOnly.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("register-admin")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto model)
    {
        await _authService.RegisterAdminAsync(model);

        return Ok(new { Status = "Success", Message = "Administrador criado com sucesso!" });
    }

    /// <summary>
    /// Recebe um e-mail e senha e, se as credenciais estiverem corretas, 
    /// retorna um token JWT para ser usado na autenticação das outras rotas.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var token = await _authService.LoginAsync(model);

        return Ok(new { token });
    }

    /// <summary>
    /// Permite que um usuário Administrador altere sua própria senha. 
    /// Ele usa o token JWT para identificar quem está logado e exige que o usuário forneça a senha atual e a nova senha.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("change-password")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        string username = User.Identity!.Name!;

        await _authService.ChangePasswordAsync(username, model);

        return Ok(new { Status = "Success", Message = "Senha alterada com sucesso!" });
    }

    /// <summary>
    /// Endpoint exclusivo de Admin para redefinir a senha de qualquer conta de usuário no sistema 
    /// sem precisar saber a senha antiga. Ele identifica o usuário-alvo pelo e-mail.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("reset-password-admin")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPasswordAdmin([FromBody] ResetPasswordDto model)
    {
        await _authService.ResetPasswordByAdminAsync(model);

        return Ok(new { Status = "Success", Message = "Senha do usuário redefinida com sucesso pelo administrador!" });
    }
}
