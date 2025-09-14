namespace Core.Application.DTOs
{
    public class CreateAlunoDto
    {
        public CreateAlunoDto()
        {
        }

        public CreateAlunoDto(string nome, string email, string cpf, DateTime dataNascimento)
        {
            Nome = nome;
            Email = email;
            Cpf = cpf;
            DataNascimento = dataNascimento;
        }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Cpf { get; set; }

        public DateTime DataNascimento { get; set; }
    }
}
