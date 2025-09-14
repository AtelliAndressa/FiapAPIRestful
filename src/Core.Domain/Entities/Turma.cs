using Core.Domain.Common;

namespace Core.Domain.Entities
{
    public class Turma : BaseEntity
    {
        public required string Nome { get; set; }

        public required string Descricao { get; set; }

        public virtual ICollection<Matricula> Matriculas { get; set; }
    }
}
