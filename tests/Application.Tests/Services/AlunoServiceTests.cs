using Core.Application.DTOs;
using Core.Application.Services;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Application.Tests.Services
{
    public class AlunoServiceTests
    {
        private readonly Mock<IAlunoRepository> _alunoRepoMock;
        private readonly Mock<IValidator<CreateAlunoDto>> _createValidatorMock;
        private readonly Mock<IValidator<UpdateAlunoDto>> _validatorMock;
        private readonly AlunoService _alunoService;

        public AlunoServiceTests()
        {
            _alunoRepoMock = new Mock<IAlunoRepository>();
            _createValidatorMock = new Mock<IValidator<CreateAlunoDto>>();
            _validatorMock = new Mock<IValidator<UpdateAlunoDto>>();

            _alunoService = new AlunoService(
                _alunoRepoMock.Object,
                _createValidatorMock.Object,
                _validatorMock.Object
            );
        }

        /// <summary>
        /// Simula o repositório encontrando um aluno e verifica se o serviço consegue mapear
        /// corretamente a entidade Aluno para um AlunoDto.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByIdAsync_WhenAlunoExists_ShouldReturnAlunoDto()
        {
            var fakeAluno = new Aluno
            {
                Id = 1,
                Nome = "Aluno Teste",
                Cpf = "12345678901",
                Email = "teste@email.com",
                DataNascimento = new DateTime(2000, 1, 1)
            };

            _alunoRepoMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(fakeAluno);

            var result = await _alunoService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.IsType<AlunoDto>(result);
            Assert.Equal("teste@email.com", result.Email);
            Assert.Equal("Aluno Teste", result.Nome);
        }

        /// <summary>
        /// Simula o repositório não encontrando um aluno e verifica se o serviço se comporta como esperado,
        /// lançando uma ValidationException (em vez de retornar nulo).
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByIdAsync_WhenAlunoDoesNotExist_ShouldThrowValidationException()
        {
            _alunoRepoMock.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Aluno)null);

            Func<Task> act = async () => await _alunoService.GetByIdAsync(99);

            var exception = await Assert.ThrowsAsync<ValidationException>(act);

            Assert.Equal("Aluno não encontrado.", exception.Message);
        }

        /// <summary>
        /// Testa a lógica de paginação.
        /// Verifica se o serviço mapeia corretamente os itens para AlunoDto,
        /// preservando todos os metadados da paginação (como TotalCount, PageNumber, etc.).
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedResults_CorrectlyMappedToDto()
        {
            var fakeAlunoList = new List<Aluno>
            {
                new Aluno { Id = 1, Nome = "Aluno Teste 1", Cpf = "111", Email = "a@a.com" }
            };

            var fakeRepoResult = new PagedResult<Aluno>(fakeAlunoList, 1, 1, 10);

            _alunoRepoMock.Setup(repo => repo.GetAllAsync(1, 10)).ReturnsAsync(fakeRepoResult);

            var result = await _alunoService.GetAllAsync(1, 10);

            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.PageNumber);
            Assert.Equal(10, result.PageSize);
            Assert.NotNull(result.Items);

            var itemUnico = Assert.Single(result.Items);

            Assert.Equal("a@a.com", itemUnico.Email);
            Assert.Equal("Aluno Teste 1", itemUnico.Nome);
            Assert.Equal(1, itemUnico.Id);
        }

        /// <summary>
        /// Simula um DTO válido (passando pelo validador) e um CPF único (repositório não encontra duplicatas) 
        /// e verifica se o serviço chama o método AddAsync do repositório para salvar o novo aluno.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WithValidDataAndUniqueCpf_ShouldSucceed()
        {
            var createDto = new CreateAlunoDto("Aluno Novo", "novo@email.com", "12345678901", DateTime.Now);

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult());

            _alunoRepoMock.Setup(r => r.GetByCpfAsync(createDto.Cpf))
                          .ReturnsAsync((Aluno)null);

            await _alunoService.AddAsync(createDto);

            _alunoRepoMock.Verify(r => r.AddAsync(It.Is<Aluno>(a => a.Nome == createDto.Nome)), Times.Once);
        }

        /// <summary>
        /// Testa a funcionalidade de busca por nome, garantindo que
        /// o serviço chame o método correto do repositório e retorne os
        /// resultados paginados e mapeados corretamente.
        /// </summary>
        [Fact]
        public async Task SearchByNameAsync_WhenMatchesExist_ShouldReturnPaginatedDtos()
        {
            var nomeBusca = "Teste";

            int pageNumber = 1;

            int pageSize = 10;

            var fakeAlunoList = new List<Aluno>
            {
                new Aluno { Id = 1, Nome = "Aluno Teste 1", Cpf = "111", Email = "a@a.com", DataNascimento = new DateTime(2000, 1, 1) },
                new Aluno { Id = 2, Nome = "Outro Teste 2", Cpf = "222", Email = "b@b.com", DataNascimento = new DateTime(2001, 1, 1) }
            };

            var fakeRepoResult = new PagedResult<Aluno>(fakeAlunoList, 2, pageNumber, pageSize);

            _alunoRepoMock.Setup(repo => repo.SearchByNameAsync(nomeBusca, pageNumber, pageSize))
                          .ReturnsAsync(fakeRepoResult);

            var result = await _alunoService.SearchByNameAsync(nomeBusca, pageNumber, pageSize);

            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(pageNumber, result.PageNumber);


            Assert.NotNull(result.Items);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal("Aluno Teste 1", result.Items[0].Nome);
            Assert.Equal("Outro Teste 2", result.Items[1].Nome);
        }

        /// <summary>
        /// Simula o repositório encontrando um aluno com o mesmo CPF e verifica se o serviço impede a criação, 
        /// lançando a ValidationException específica com a mensagem de erro correta.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WhenCpfAlreadyExists_ShouldThrowValidationException()
        {
            var createDto = new CreateAlunoDto("Aluno Fantasma", "fantasma@email.com", "111222333", DateTime.Now);
            var existingAluno = new Aluno { Id = 99, Nome = "Usuario Antigo", Cpf = "111222333", Email = "teste1@gmail.com"};

            _createValidatorMock.Setup(v => v.ValidateAsync(createDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult());

            _alunoRepoMock.Setup(r => r.GetByCpfAsync(createDto.Cpf))
                          .ReturnsAsync(existingAluno);

            Func<Task> act = async () => await _alunoService.AddAsync(createDto);

            var exception = await Assert.ThrowsAsync<ValidationException>(act);

            Assert.Equal("Já existe um aluno cadastrado com este CPF.", exception.Message);

            _alunoRepoMock.Verify(r => r.AddAsync(It.IsAny<Aluno>()), Times.Never);
        }

        /// <summary>
        /// Simula o validador retornando um erro (ex: nome muito curto) e verifica se o serviço lança 
        /// a exceção imediatamente, sem tentar fazer nenhuma consulta ao banco de dados (como checar CPF ou adicionar).
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task AddAsync_WhenDtoIsInvalid_ShouldThrowValidationException()
        {
            var invalidDto = new CreateAlunoDto("A", "", "123", DateTime.Now);
            var validationErrors = new List<ValidationFailure> { new ValidationFailure("Nome", "Nome muito curto") };

            _createValidatorMock.Setup(v => v.ValidateAsync(invalidDto, It.IsAny<CancellationToken>()))
                                .ReturnsAsync(new ValidationResult(validationErrors));

            Func<Task> act = async () => await _alunoService.AddAsync(invalidDto);

            await act.Should().ThrowAsync<ValidationException>();

            _alunoRepoMock.Verify(r => r.GetByCpfAsync(It.IsAny<string>()), Times.Never);
            _alunoRepoMock.Verify(r => r.AddAsync(It.IsAny<Aluno>()), Times.Never);
        }
    }
}