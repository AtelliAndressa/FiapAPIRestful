using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class Matricula : BaseEntity
    {
        public Guid AlunoId { get; set; }

        public Guid TurmaId { get; set; }

        public DateTime DataMatricula { get; set; }

        public Aluno Aluno { get; set; } = null!;

        public Turma Turma { get; set; } = null!;
    }
}
