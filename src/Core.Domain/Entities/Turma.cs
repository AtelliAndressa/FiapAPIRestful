namespace Core.Domain.Entities
{
    public class Turma
    {
        public Guid Id { get; set; }

        public required string Nome { get; set; }

        public required string Descricao { get; set; }

        public virtual ICollection<Matricula> Matriculas { get; set; }
    }
}
