using Core.Application.DTOs;
using Core.Application.Services;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests.Services;

public class AlunoServiceTests
{
    [Fact]
    public async Task GetAllAsync_ShouldReturnListOfAlunoDto_WhenAlunosExist()
    {
        var mockAlunos = new List<Aluno>
        {
            new() { Id = 1, Nome = "João Silva", Cpf = "11122233344", Email = "joao@gmail.com", DataNascimento = new DateTime(2000, 1, 10) },
            new() { Id = 2, Nome = "Maria Souza", Cpf = "55566677788", Email = "maria@gmail.com", DataNascimento = new DateTime(2001, 2, 2) }
        };

        var mockRepo = new Mock<IAlunoRepository>();
        mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(mockAlunos);

        var alunoService = new AlunoService(mockRepo.Object);

        var result = await alunoService.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<AlunoDto>();
        mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}