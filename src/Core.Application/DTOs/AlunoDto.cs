namespace Core.Application.DTOs
{
    public class AlunoDto
    {
        public AlunoDto(Guid id, string nome, string cpf, string email, DateTime dataNascimento)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
            Email = email;
            DataNascimento = dataNascimento;
        }

        public Guid Id { get; set; }

        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string Email { get; set; }

        public DateTime DataNascimento { get; set; }
    }
}