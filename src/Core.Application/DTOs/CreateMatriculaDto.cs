namespace Core.Application.DTOs
{
    public class CreateMatriculaDto
    {
        public CreateMatriculaDto()
        {
        }

        public CreateMatriculaDto(int alunoId, int turmaId, DateTime dataMatricula)
        {
            AlunoId = alunoId;
            TurmaId = turmaId;
            DataMatricula = dataMatricula;
        }

        public int AlunoId { get; set; }
        public int TurmaId { get; set; }
        public DateTime DataMatricula { get; set; }
    }
}
