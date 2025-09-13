namespace Core.Domain.Entities
{
    public class Matricula
    {
        public int Id { get; set; }

        public int AlunoId { get; set; }

        public int TurmaId { get; set; }

        public DateTime DataMatricula { get; set; }

        public Aluno Aluno { get; set; } = null!;

        public Turma Turma { get; set; } = null!;
    }
}
