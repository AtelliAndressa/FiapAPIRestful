namespace Core.Application.DTOs
{
    public class RegisterAdminDto
    {
        public RegisterAdminDto()
        {
        }

        public RegisterAdminDto(string email, string password, string confirmPassword)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
