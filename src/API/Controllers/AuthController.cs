using Core.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Cadastra um novo usuario administrador no sistema.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("register-admin")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Email);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status409Conflict,
                new { Status = "Error", Message = "Já existe um usuário com este email!" });
        }

        var newUser = new IdentityUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { Status = "Error", Errors = result.Errors.Select(e => e.Description) });
        }

        var roleResult = await _userManager.AddToRoleAsync(newUser, "Admin");

        if (!roleResult.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Status = "Error", Message = "Falha ao atribuir o perfil de administrador ao usuário." });
        }

        return Ok(new { Status = "Success", Message = "Administrador criado com sucesso!" });
    }

    /// <summary>
    /// Cadastra um novo usuario comum no sistema.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("register-user")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Email);
        if (userExists != null)
        {
            return StatusCode(StatusCodes.Status409Conflict,
                new { Status = "Error", Message = "Já existe um usuário com este email!" });
        }

        var newUser = new IdentityUser()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new { Status = "Error", Errors = result.Errors.Select(e => e.Description) });
        }

        var roleResult = await _userManager.AddToRoleAsync(newUser, "User");

        if (!roleResult.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Status = "Error", Message = "Falha ao atribuir o perfil de usuário." });
        }

        return Ok(new { Status = "Success", Message = "Usuário criado com sucesso!" });
    }

    /// <summary>
    /// Realiza o login de um usuário válido e gera um token JWT.
    /// </summary>
    /// <remarks>
    /// Este método valida as credenciais fornecidas (usuário e senha) e, caso sejam válidas,
    /// retorna um token JWT que poderá ser utilizado para acessar reTurmas protegidos da API.
    /// 
    /// O token gerado expira em 3 horas a partir da autenticação.
    /// </remarks>
    /// <param name="model">Modelo contendo o nome de usuário e a senha para autenticação.</param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo o token JWT em caso de sucesso;
    /// caso contrário, retorna <see cref="UnauthorizedResult"/>.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        return Unauthorized();
    }

    /// <summary>
    /// Permite que o usuário autenticado altere sua própria senha.
    /// </summary>
    /// <remarks>
    /// O usuário deve fornecer sua senha atual para verificação, além da nova senha e sua confirmação.
    /// </remarks>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        if (model.NewPassword != model.ConfirmNewPassword)
        {
            return BadRequest(new { Status = "Error", Message = "A nova senha e a confirmação não conferem." });
        }

        string username = User.Identity.Name;

        IdentityUser? user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return NotFound(new { Status = "Error", Message = "Usuário não encontrado." });
        }

        IdentityResult result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(new { Status = "Error", Errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { Status = "Success", Message = "Senha alterada com sucesso!" });
    }

    /// <summary>
    /// Permite que um administrador redefina a senha de qualquer usuário.
    /// </summary>
    /// <remarks>
    /// Esta é uma operação privilegiada e só pode ser executada por um administrador.
    /// </remarks>
    [HttpPost("reset-password-admin")]
    [Authorize(Policy = "AdminOnly")] 
    public async Task<IActionResult> ResetPasswordAdmin([FromBody] ResetPasswordDto model)
    {
        if (model.NewPassword != model.ConfirmNewPassword)
        {
            return BadRequest(new { Status = "Error", Message = "A nova senha e a confirmação não conferem." });
        }

        IdentityUser? user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
        {
            return NotFound(new { Status = "Error", Message = "Usuário com o email especificado não foi encontrado." });
        }

        string token = await _userManager.GeneratePasswordResetTokenAsync(user);

        IdentityResult result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(new { Status = "Error", Errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { Status = "Success", Message = "Senha do usuário redefinida com sucesso pelo administrador!" });
    }
}