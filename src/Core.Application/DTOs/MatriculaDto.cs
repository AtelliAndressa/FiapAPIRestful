namespace Core.Application.DTOs;

public record MatriculaDto(int Id, int AlunoId, int CursoId, DateTime DataMatricula);

public record CreateMatriculaDto(int AlunoId, int CursoId, DateTime DataMatricula);



