namespace Core.Application.DTOs
{
    public class ResetPasswordDto
    {
        public ResetPasswordDto()
        {
        }

        public ResetPasswordDto(string email, string newPassword, string confirmNewPassword)
        {
            Email = email;
            NewPassword = newPassword;
            ConfirmNewPassword = confirmNewPassword;
        }

        public string Email { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;

        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
