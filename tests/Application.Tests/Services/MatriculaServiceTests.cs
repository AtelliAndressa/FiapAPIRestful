using Core.Application.DTOs;
using Core.Application.Services;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentAssertions;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Application.Tests.Services
{
    public class MatriculaServiceTests
    {
        private readonly Mock<IMatriculaRepository> _matriculaRepoMock;
        private readonly Mock<IAlunoRepository> _alunoRepoMock;
        private readonly Mock<ITurmaRepository> _turmaRepoMock;
        private readonly MatriculaService _service; // O Serviço REAL que estamos testando

        // Construtor: Este código roda antes de CADA teste
        public MatriculaServiceTests()
        {
            // Arrange (Configuração inicial para todos os testes)
            _matriculaRepoMock = new Mock<IMatriculaRepository>();
            _alunoRepoMock = new Mock<IAlunoRepository>();
            _turmaRepoMock = new Mock<ITurmaRepository>();

            // Instanciamos o serviço real, injetando os Mocks (dublês)
            _service = new MatriculaService(
                _matriculaRepoMock.Object,
                _alunoRepoMock.Object,
                _turmaRepoMock.Object
            );
        }

        [Fact]
        public async Task CreateAsync_WithValidData_ShouldSucceedAndReturnRichDto()
        {
            int pageNumber = 1;
            int pageSize = 10;
            // 1. Criar dados de entrada falsos
            var createDto = new CreateMatriculaDto(1, 1, DateTime.Now); // Assumindo o DTO que definimos
            var fakeAluno = new Aluno { Id = 1, Nome = "Aluno Teste", Cpf = "12345678901", DataNascimento = DateTime.Now };
            var fakeTurma = new Turma { Id = 1, Nome = "Turma Teste", Descricao = "Desc" };

            // 2. Programar os Mocks: "Dizer" aos repositórios falsos o que retornar

            // "Quando o serviço perguntar pelo aluno com Id 1, retorne o aluno falso"
            _alunoRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(fakeAluno);

            // "Quando o serviço perguntar pela turma com Id 1, retorne a turma falsa"
            _turmaRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(fakeTurma);

            // "Quando o serviço perguntar se o aluno já está matriculado (RF05), retorne 'false'"
            _matriculaRepoMock.Setup(repo => repo.IsStudentAlreadyEnrolledAsync(1, 1)).ReturnsAsync(false);

            // --- ACT ---

            // 3. Executar o método
            var result = await _service.CreateAsync(createDto);

            // --- ASSERT ---

            // 4. Verificar os resultados com FluentAssertions
            result.Should().NotBeNull();
            result.Should().BeOfType<MatriculaDto>();
            result.Aluno.Id.Should().Be(1);
            result.Aluno.Nome.Should().Be("Aluno Teste");
            result.Turma.Nome.Should().Be("Turma Teste");

            // 5. Verificar se o método AddAsync do repositório foi chamado EXATAMENTE UMA VEZ
            _matriculaRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Matricula>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WhenStudentIsAlreadyEnrolled_ShouldThrowValidationException()
        {
            [cite_start]// Este teste valida especificamente o Requisito RF05 

            // --- ARRANGE ---
            var createDto = new CreateMatriculaDto(1, 1, DateTime.Now);
            var fakeAluno = new Aluno { Id = 1 };
            var fakeTurma = new Turma { Id = 1 };

            // Configuração dos mocks para "passar" pelas primeiras validações
            _alunoRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(fakeAluno);
            _turmaRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(fakeTurma);

            // AQUI ESTÁ O PONTO-CHAVE DO TESTE:
            // "Quando o serviço perguntar se o aluno já está matriculado, retorne 'true'"
            _matriculaRepoMock.Setup(repo => repo.IsStudentAlreadyEnrolledAsync(1, 1)).ReturnsAsync(true);

            // --- ACT ---

            // Para testar exceções, precisamos encapsular a chamada do método em uma Action ou Func
            Func<Task> act = async () => await _service.CreateAsync(createDto);

            // --- ASSERT ---

            // Verificamos se a ação (act) lança EXATAMENTE a exceção que esperamos
            // E se a mensagem da exceção é a que definimos no serviço.
            await act.Should().ThrowAsync<ValidationException>()
                     .WithMessage("O aluno já está matriculado nesta turma.");

            // Também garantimos que o método AddAsync NUNCA foi chamado
            _matriculaRepoMock.Verify(repo => repo.AddAsync(It.IsAny<Matricula>()), Times.Never);
        }
    }
}