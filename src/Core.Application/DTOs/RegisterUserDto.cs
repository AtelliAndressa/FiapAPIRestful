namespace Core.Application.DTOs
{
    public class RegisterUserDto
    {
        public RegisterUserDto()
        {
        }

        public RegisterUserDto(string email, string password, string confirmPassword)
        {
            Email = email;
            Password = password;
            ConfirmPassword = confirmPassword;
        }

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
