namespace Core.Application.DTOs
{
    public class MatriculaDto
    {
        public MatriculaDto()
        {
        }

        public MatriculaDto(Guid id, AlunoDto alunoDto, TurmaDto turmaDto, DateTime dataMatricula)
        {
            Id = id;
            this.Aluno = alunoDto;
            this.Turma = turmaDto;
            DataMatricula = dataMatricula;
        }

        public Guid Id { get; set; }

        public AlunoDto Aluno { get; set; } = new AlunoDto();

        public TurmaDto Turma { get; set; } = new TurmaDto();

        public DateTime DataMatricula { get; set; }
    }
}