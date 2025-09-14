using Core.Application.DTOs;
using Core.Application.Services;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Application.Tests.Services
{
    public class MatriculaServiceTests
    {
        private readonly Mock<IMatriculaRepository> _matriculaRepoMock;
        private readonly Mock<IAlunoRepository> _alunoRepoMock;
        private readonly Mock<ITurmaRepository> _turmaRepoMock;
        private readonly Mock<IValidator<CreateMatriculaDto>> _createValidatorMock;
        private readonly Mock<IValidator<MatriculaDto>> _validatorMock;

        private readonly MatriculaService _matriculaService;

        public MatriculaServiceTests()
        {
            _matriculaRepoMock = new Mock<IMatriculaRepository>();
            _alunoRepoMock = new Mock<IAlunoRepository>();
            _turmaRepoMock = new Mock<ITurmaRepository>();
            _createValidatorMock = new Mock<IValidator<CreateMatriculaDto>>();
            _validatorMock = new Mock<IValidator<MatriculaDto>>();

            _matriculaService = new MatriculaService(
                _matriculaRepoMock.Object,
                _alunoRepoMock.Object,
                _turmaRepoMock.Object,
                _createValidatorMock.Object,
                _validatorMock.Object
            );
        }

        /// <summary>
        /// Simula um cenário onde o DTO é válido, o aluno existe, a turma existe e a matrícula é nova, 
        /// verificando se o serviço retorna o DTO "rico" e chama o método de salvar no repositório.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WithValidData_ShouldSucceedAndReturnRichDto()
        {
            var createDto = new CreateMatriculaDto(1, 1, DateTime.Now);

            var fakeAluno = new Aluno
            {
                Id = 1,
                Nome = "Aluno Teste",
                Cpf = "22580591866",
                Email = "testeAluno@gmail.com",
                DataNascimento = new DateTime(1983, 12, 14)
            };

            var fakeTurma = new Turma { Id = 1, Nome = "Turma Teste", Descricao = "Desc" };

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult());

            _alunoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeAluno);

            _turmaRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeTurma);

            _matriculaRepoMock.Setup(r => r.IsStudentAlreadyEnrolledAsync(1, 1)).ReturnsAsync(false);

            var result = await _matriculaService.AddAsync(createDto);

            Assert.NotNull(result);
            Assert.IsType<MatriculaDto>(result);
            Assert.Equal("Aluno Teste", result.Aluno.Nome);
            Assert.Equal("Turma Teste", result.Turma.Nome);

            _matriculaRepoMock.Verify(r => r.AddAsync(It.IsAny<Matricula>()), Times.Once);
        }

        /// <summary>
        /// Simula que o aluno já está matriculado (o mock IsStudentAlreadyEnrolledAsync retorna true) e verifica se o 
        /// serviço lança a ValidationException correta, impedindo a matrícula duplicada.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WhenStudentAlreadyEnrolled_ShouldThrowValidationException()
        {
            var createDto = new CreateMatriculaDto(1, 1, new DateTime(2025, 1, 10));

            var fakeAluno = new Aluno
            {
                Id = 1,
                Nome = "Aluno Teste Real",
                Cpf = "12345678901",
                Email = "aluno@teste.com",
                DataNascimento = new DateTime(2000, 10, 14)
            };

            var fakeTurma = new Turma { Id = 1, Nome = "Turma Teste Real", Descricao = "Desc Real" };

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult());

            _alunoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeAluno);

            _turmaRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeTurma);

            _matriculaRepoMock.Setup(r => r.IsStudentAlreadyEnrolledAsync(1, 1)).ReturnsAsync(true);

            Func<Task> act = async () => await _matriculaService.AddAsync(createDto);

            var exception = await Assert.ThrowsAsync<ValidationException>(act);

            Assert.Equal("O aluno já está matriculado nesta turma.", exception.Message);

            _matriculaRepoMock.Verify(r => r.AddAsync(It.IsAny<Matricula>()), Times.Never);
        }

        /// <summary>
        /// Simula o IAlunoRepository retornando null e verifica se o serviço lança a ValidationException correta 
        /// informando que o aluno não foi encontrado.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WhenAlunoNotFound_ShouldThrowValidationException()
        {
            var createDto = new CreateMatriculaDto(99, 1, DateTime.Now);

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult());

            _alunoRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Aluno)null);

            Func<Task> act = async () => await _matriculaService.AddAsync(createDto);

            var exception = await Assert.ThrowsAsync<ValidationException>(act);

            Assert.Equal("Aluno não encontrado. Por favor cadastre o Aluno antes de efetuar a matrícula.", exception.Message);

            _matriculaRepoMock.Verify(r => r.AddAsync(It.IsAny<Matricula>()), Times.Never);
        }

        /// <summary>
        /// Simula que o aluno foi encontrado, mas a ITurmaRepository retorna null, e verifica se o 
        /// serviço lança a ValidationException correta informando que a turma não foi encontrada.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WhenTurmaNotFound_ShouldThrowValidationException()
        {
            var createDto = new CreateMatriculaDto(1, 99, DateTime.Now);

            var fakeAluno = new Aluno
            {
                Id = 1,
                Nome = "Aluno Teste Real",
                Cpf = "12345678901",
                Email = "aluno@teste.com",
                DataNascimento = new DateTime(2000, 1, 1)
            };

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult());

            _alunoRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeAluno);

            _turmaRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Turma)null);

            Func<Task> act = async () => await _matriculaService.AddAsync(createDto);

            var exception = await Assert.ThrowsAsync<ValidationException>(act);

            Assert.Equal("Turma não encontrada. Por favor cadastre a turma antes de efetuar a matrícula.", exception.Message);

            _matriculaRepoMock.Verify(r => r.AddAsync(It.IsAny<Matricula>()), Times.Never);
        }

        /// <summary>
        /// Simula o repositório retornando um PagedResult<Matricula> (entidades com Aluno/Turma) e verifica se o 
        /// serviço mapeia corretamente para um PagedResult<MatriculaDto> (DTOs), preservando os dados da paginação.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedRichDtos()
        {
            var fakeAluno = new Aluno
            {
                Id = 1,
                Nome = "Aluno Teste Real",
                Cpf = "12345678901",
                Email = "aluno@teste.com",
                DataNascimento = new DateTime(2001, 12, 1)
            };

            var fakeTurma = new Turma { Id = 1, Nome = "Turma Teste Real", Descricao = "Desc Real" };

            var fakeMatriculaList = new List<Matricula>
            {
                new Matricula { Id = 1, Aluno = fakeAluno, Turma = fakeTurma, DataMatricula = DateTime.Now }
            };

            var fakeRepoResult = new PagedResult<Matricula>(fakeMatriculaList, 1, 1, 10);

            _matriculaRepoMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(fakeRepoResult);

            var result = await _matriculaService.GetAllAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);

            var item = Assert.Single(result.Items);

            Assert.Equal("Aluno Teste Real", item.Aluno.Nome);
        }
    }
}