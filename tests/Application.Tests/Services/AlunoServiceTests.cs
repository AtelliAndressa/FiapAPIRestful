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
        private readonly AlunoService _alunoService;

        private readonly Guid _fakeAlunoId = Guid.NewGuid();

        public AlunoServiceTests()
        {
            _alunoRepoMock = new Mock<IAlunoRepository>();
            _alunoService = new AlunoService(
                _alunoRepoMock.Object
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
                Id = _fakeAlunoId,
                Nome = "Aluno Teste",
                Cpf = "12345678901",
                Email = "teste@email.com",
                DataNascimento = new DateTime(2000, 1, 1)
            };

            _alunoRepoMock.Setup(repo => repo.GetByIdAsync(_fakeAlunoId)).ReturnsAsync(fakeAluno);

            var result = await _alunoService.GetByIdAsync(_fakeAlunoId);

            Assert.NotNull(result);
            Assert.IsType<AlunoDto>(result);
            Assert.Equal("teste@email.com", result.Email);
            Assert.Equal("Aluno Teste", result.Nome);
        }

        /// <summary>
        /// Simula o repositório não encontrando um aluno e verifica se o serviço se comporta como esperado.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetByIdAsync_WhenAlunoDoesNotExist_ShouldThrowValidationException()
        {
            var fakeGuid = Guid.NewGuid();

            _alunoRepoMock.Setup(repo => repo.GetByIdAsync(fakeGuid))
                          .ReturnsAsync((Aluno)null);

            var result = await _alunoService.GetByIdAsync(fakeGuid);

            Assert.Null(result);
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
                new Aluno { Id = _fakeAlunoId, Nome = "Aluno Teste 1", Cpf = "111", Email = "a@a.com" }
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
            Assert.Equal(_fakeAlunoId, itemUnico.Id);
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
                new Aluno { Id = _fakeAlunoId, Nome = "Aluno Teste 1", Cpf = "111", Email = "a@a.com", DataNascimento = new DateTime(2000, 1, 1) },
                new Aluno { Id = _fakeAlunoId, Nome = "Outro Teste 2", Cpf = "222", Email = "b@b.com", DataNascimento = new DateTime(2001, 1, 1) }
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
            var existingAluno = new Aluno { Id = _fakeAlunoId, Nome = "Usuario Antigo", Cpf = "111222333", Email = "teste1@gmail.com"};

            _alunoRepoMock.Setup(r => r.GetByCpfAsync(createDto.Cpf))
                          .ReturnsAsync(existingAluno);

            Func<Task> act = async () => await _alunoService.AddAsync(createDto);

            var exception = await Assert.ThrowsAsync<ValidationException>(act);

            Assert.Equal("Já existe um aluno cadastrado com este CPF.", exception.Message);

            _alunoRepoMock.Verify(r => r.AddAsync(It.IsAny<Aluno>()), Times.Never);
        }
    }
}