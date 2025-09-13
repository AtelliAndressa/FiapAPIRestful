namespace Core.Application.DTOs;

public record TurmaDto(int Id, string Nome, string Descricao);

public record CreateTurmaDto(string Nome, string Descricao);

