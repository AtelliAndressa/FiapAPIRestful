namespace Core.Application.DTOs;

public record RegisterAdminDto(string Email, string Password, string ConfirmPassword);
