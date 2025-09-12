namespace Core.Application.DTOs;

public record AlunoDto(int Id, string Nome, string Cpf, string Email, DateTime DataNascimento);

public record CreateAlunoDto(string Nome, string Cpf, string Email, DateTime DataNascimento);
