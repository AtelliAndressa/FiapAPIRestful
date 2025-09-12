namespace Core.Application.DTOs;

public record RegisterUserDto(string Email, string Password, string ConfirmPassword);