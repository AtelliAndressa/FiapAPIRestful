namespace Core.Application.DTOs
{
    public class CreateMatriculaDto
    {
        public CreateMatriculaDto()
        {
        }

        public CreateMatriculaDto(Guid alunoId, Guid turmaId, DateTime dataMatricula)
        {
            AlunoId = alunoId;
            TurmaId = turmaId;
            DataMatricula = dataMatricula;
        }

        public Guid AlunoId { get; set; }

        public Guid TurmaId { get; set; }

        public DateTime DataMatricula { get; set; }
    }
}
