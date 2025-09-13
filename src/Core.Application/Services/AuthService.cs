using Core.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task RegisterAdminAsync(RegisterAdminDto model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Email);

        if (userExists != null)
            throw new Exception("Já existe um usuário com este email!");

        var newUser = new IdentityUser
        {
            Email = model.Email,
            UserName = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        var roleResult = await _userManager.AddToRoleAsync(newUser, "Admin");

        if (!roleResult.Succeeded)
            throw new Exception("Falha ao atribuir o perfil de administrador ao usuário.");
    }

    public async Task RegisterUserAsync(RegisterUserDto model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Email);

        if (userExists != null)
            throw new Exception("Já existe um usuário com este email!");

        var newUser = new IdentityUser
        {
            Email = model.Email,
            UserName = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        var roleResult = await _userManager.AddToRoleAsync(newUser, "User");

        if (!roleResult.Succeeded)
            throw new Exception("Falha ao atribuir o perfil de usuário.");
    }

    public async Task<string> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            throw new UnauthorizedAccessException("Usuário ou senha inválidos.");

        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task ChangePasswordAsync(string username, ChangePasswordDto model)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
            throw new Exception("Usuário não encontrado.");

        var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task ResetPasswordByAdminAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user == null)
            throw new Exception("Usuário não encontrado.");

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
