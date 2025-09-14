using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentValidation;

namespace Core.Application.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IAlunoRepository _alunoRepository;

        public AlunoService(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        public async Task AddAsync(CreateAlunoDto alunoDto)
        {
            Aluno alunoExists = await _alunoRepository.GetByCpfAsync(alunoDto.Cpf);

            if (alunoExists != null)
            {
                throw new ValidationException("Já existe um aluno cadastrado com este CPF.");
            }

            Aluno aluno = new Aluno
            {
                Nome = alunoDto.Nome,
                Email = alunoDto.Email,
                Cpf = alunoDto.Cpf,
                DataNascimento = alunoDto.DataNascimento
            };

            await _alunoRepository.AddAsync(aluno);
        }

        public async Task<AlunoDto> GetByIdAsync(Guid id)
        {
            Aluno aluno = await _alunoRepository.GetByIdAsync(id);

            if (aluno == null)
            {
                return null;
            }

            return new AlunoDto(aluno.Id, aluno.Nome, aluno.Cpf, aluno.Email, aluno.DataNascimento);
        }

        public async Task UpdateAsync(Guid id, UpdateAlunoDto alunoDto)
        {
            Aluno aluno = await _alunoRepository.GetByIdAsync(id);

            if (aluno == null)
            {
                throw new ValidationException("Aluno não encontrado.");
            }

            aluno.Nome = alunoDto.Nome;
            aluno.Email = alunoDto.Email;
            aluno.DataNascimento = alunoDto.DataNascimento;

            await _alunoRepository.UpdateAsync(aluno);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            Aluno aluno = await _alunoRepository.GetByIdAsync(id);

            if (aluno == null)
            {
                return false;
            }

            await _alunoRepository.DeleteAsync(aluno.Id);

            return true;
        }

        public async Task<PagedResult<AlunoDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            PagedResult<Aluno> pagedResultEntity = await _alunoRepository.GetAllAsync(pageNumber, pageSize);

            List<AlunoDto> itemsDto = pagedResultEntity.Items
                .Select(a => new AlunoDto(a.Id, a.Nome, a.Cpf, a.Email, a.DataNascimento))
                .ToList();

            return new PagedResult<AlunoDto>(
                itemsDto,
                pagedResultEntity.TotalCount,
                pagedResultEntity.PageNumber,
                pagedResultEntity.PageSize
            );
        }

        public async Task<PagedResult<AlunoDto>> SearchByNameAsync(string nome, int pageNumber, int pageSize)
        {
            PagedResult<Aluno> pagedResultEntity = await _alunoRepository.SearchByNameAsync(nome, pageNumber, pageSize);

            List<AlunoDto> itemsDto = pagedResultEntity.Items
                .Select(a => new AlunoDto(a.Id, a.Nome, a.Cpf, a.Email, a.DataNascimento))
                .ToList();

            return new PagedResult<AlunoDto>(
                itemsDto,
                pagedResultEntity.TotalCount,
                pagedResultEntity.PageNumber,
                pagedResultEntity.PageSize
            );
        }
    }
}
