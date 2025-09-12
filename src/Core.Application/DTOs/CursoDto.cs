namespace Core.Application.DTOs;

public record CursoDto(int Id, string Nome, string Descricao);

public record CreateCursoDto(string Nome, string Descricao);

