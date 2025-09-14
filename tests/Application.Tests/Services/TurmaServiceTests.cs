using Xunit;
using Moq;
using Core.Application.Services;
using Core.Domain.Interfaces;
using Core.Domain.Entities;
using Core.Application.DTOs;
using Core.Domain.Common;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Tests.Services
{
    public class TurmaServiceTests
    {
        private readonly Mock<ITurmaRepository> _turmaRepoMock;
        private readonly Mock<IValidator<CreateTurmaDto>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateTurmaDto>> _validatorMock;
        private readonly TurmaService _turmaService;

        public TurmaServiceTests()
        {
            _turmaRepoMock = new Mock<ITurmaRepository>();
            _createValidatorMock = new Mock<IValidator<CreateTurmaDto>>();
            _validatorMock = new Mock<IValidator<UpdateTurmaDto>>();

            _turmaService = new TurmaService(
                _turmaRepoMock.Object,
                _createValidatorMock.Object,
                _validatorMock.Object
            );
        }

        /// <summary>
        /// Simula que o repositório encontrou uma turma com o ID solicitado e verifica se o serviço mapeia 
        /// corretamente a entidade Turma para o TurmaDto de retorno.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByIdAsync_WhenTurmaExists_ShouldReturnTurmaDto()
        {
            var fakeTurma = new Turma { Id = 1, Nome = "Turma A", Descricao = "Desc A" };

            _turmaRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(fakeTurma);

            var result = await _turmaService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.IsType<TurmaDto>(result);
            Assert.Equal("Turma A", result.Nome);
            Assert.Equal(1, result.Id);
        }

        /// <summary>
        /// Simula o repositório não encontrando a turma (retornando null) e verifica se o serviço 
        /// lança a ValidationException.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByIdAsync_WhenTurmaDoesNotExist_ShouldThrowValidationException()
        {
            _turmaRepoMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Turma)null);

            Func<Task> act = async () => await _turmaService.GetByIdAsync(99);

            var exception = await Assert.ThrowsAsync<ValidationException>(act);

            Assert.Equal("Turma não encontrada.", exception.Message);
        }

        /// <summary>
        /// simula o repositório retornando um PagedResult<Turma> (com entidades) e verifica se o serviço 
        /// mapeia corretamente esse resultado para um PagedResult<TurmaDto> (com DTOs), preservando 
        /// todos os metadados da paginação (como TotalCount).
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults_CorrectlyMapped()
        {
            var fakeTurmaList = new List<Turma>
            {
                new Turma { Id = 1, Nome = "Turma A", Descricao = "Desc A", Matriculas = new List<Matricula>() }
            };

            var fakeRepoResult = new PagedResult<Turma>(fakeTurmaList, 1, 1, 10);

            _turmaRepoMock.Setup(repo => repo.GetAllAsync(1, 10)).ReturnsAsync(fakeRepoResult);

            var result = await _turmaService.GetAllAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);

            var item = Assert.Single(result.Items);

            Assert.Equal("Turma A", item.Nome);
            Assert.Equal(0, item.QuantidadeAlunos);
        }

        /// <summary>
        ///  Simula duas condições: 1) que o DTO de entrada passou na validação do FluentValidation e 
        ///  2) que o nome da turma é único (o mock do IsTeamAsync retorna false). O teste então verifica se o 
        ///  método AddAsync do repositório foi chamado com sucesso.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WithValidDataAndUniqueName_ShouldSucceed()
        {
            var createDto = new CreateTurmaDto("Turma Nova", "Desc Nova");

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult());

            _turmaRepoMock.Setup(r => r.IsTeamAsync(createDto.Nome)).ReturnsAsync(false);

            await _turmaService.AddAsync(createDto);

            _turmaRepoMock.Verify(
                r => r.AddAsync(It.Is<Turma>(t => t.Nome == createDto.Nome && t.Descricao == createDto.Descricao)),
                Times.Once
            );
        }

        /// <summary>
        /// Simula que o DTO de entrada é válido, mas diz ao mock do repositório que o nome da turma 
        /// já existe (IsTeamAsync retorna true). O teste então verifica se o serviço lança a ValidationException 
        /// correta e se o método AddAsync do repositório nunca foi chamado.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WhenTurmaNameExists_ShouldThrowValidationException()
        {
            var createDto = new CreateTurmaDto("Turma Repetida", "Desc");

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult());

            _turmaRepoMock.Setup(r => r.IsTeamAsync(createDto.Nome)).ReturnsAsync(true);

            Func<Task> act = async () => await _turmaService.AddAsync(createDto);

            var exception = await Assert.ThrowsAsync<ValidationException>(act);

            Assert.Equal("Já existe uma turma com este nome.", exception.Message);

            _turmaRepoMock.Verify(r => r.AddAsync(It.IsAny<Turma>()), Times.Never);
        }
    }
}