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

        public string Email { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
}
