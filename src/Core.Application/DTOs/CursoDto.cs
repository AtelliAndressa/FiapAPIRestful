namespace Core.Application.DTOs;

public record CursoDto(int Id, string Nome, string Descricao, int CargaHoraria);
public record CreateCursoDto(string Nome, string Descricao, int CargaHoraria);

