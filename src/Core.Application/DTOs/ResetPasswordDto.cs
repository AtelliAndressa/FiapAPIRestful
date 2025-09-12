namespace Core.Application.DTOs;

public record ResetPasswordDto(string Email, string NewPassword, string ConfirmNewPassword);