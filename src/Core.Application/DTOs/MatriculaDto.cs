namespace Core.Application.DTOs
{
    public class MatriculaDto
    {
        public MatriculaDto(Guid id, AlunoDto alunoDto, TurmaDto turmaDto, DateTime dataMatricula)
        {
            Id = id;
            this.Aluno = alunoDto;
            this.Turma = turmaDto;
            DataMatricula = dataMatricula;
        }

        public Guid Id { get; set; }

        public AlunoDto Aluno { get; set; }

        public TurmaDto Turma { get; set; }

        public DateTime DataMatricula { get; set; }
    }
}