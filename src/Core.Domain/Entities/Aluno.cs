namespace Core.Domain.Entities
{
    public class Aluno
    {
        public Guid Id { get; set; }

        public required string Nome { get; set; }

        public required string Cpf { get; set; }

        public required string Email { get; set; }

        public DateTime DataNascimento { get; set; }
    }
}
