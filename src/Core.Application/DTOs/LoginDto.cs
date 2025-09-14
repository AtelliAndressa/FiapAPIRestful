namespace Core.Application.DTOs
{
    public class LoginDto
    {
        public LoginDto()
        {
        }

        public LoginDto(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
