namespace Core.Application.DTOs;

public record MatriculaDto(int Id, AlunoDto Aluno, TurmaDto Turma, DateTime DataMatricula);


public record CreateMatriculaDto(int AlunoId, int TurmaId, DateTime DataMatricula);



