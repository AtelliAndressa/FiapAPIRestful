namespace Core.Application.DTOs
{
    public class AlunoDto
    {
        public AlunoDto()
        { 
        }

        public AlunoDto(int id, string nome, string cpf, string email, DateTime dataNascimento)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Email = email;
            DataNascimento = dataNascimento;
        }

        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
    }
}