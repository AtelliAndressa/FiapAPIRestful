using Core.Application.DTOs;

public interface IAuthService
{
    Task RegisterAdminAsync(RegisterAdminDto model);

    Task RegisterUserAsync(RegisterUserDto model);

    Task<string> LoginAsync(LoginDto model);

    Task ChangePasswordAsync(string username, ChangePasswordDto model);

    Task ResetPasswordByAdminAsync(ResetPasswordDto model);
}

