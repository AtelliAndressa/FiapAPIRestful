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
    /// retorna um token JWT que poderá ser utilizado para acessar recursos protegidos da API.
    /// 
    /// O token gerado expira em 3 horas a partir da autenticação.
    /// </remarks>
    /// <param name="model">Modelo contendo o nome de usuário e a senha para autenticação.</param>
    /// <returns>
    /// Um <see cref="IActionResult"/> contendo o token JWT em caso de sucesso;
    /// caso contrário, retorna <see cref="UnauthorizedResult"/>.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
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
}

public record LoginModel(string Username, string Password);