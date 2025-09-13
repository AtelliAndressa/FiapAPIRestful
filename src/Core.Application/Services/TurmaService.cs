using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Services
{
    public class TurmaService : ITurmaService
    {
        private readonly ITurmaRepository _turmaRepository;

        public TurmaService(ITurmaRepository turmaRepository)
        {
            _turmaRepository = turmaRepository;
        }

        public async Task AddAsync(TurmaDto turmaDto)
        {
            bool turmaExists = await _turmaRepository.IsTeamAsync(turmaDto.Nome);

            if (turmaExists)
            {
                throw new Exception("Já existe uma turma com este nome.");
            }

            Turma turma = new Turma
            {
                Nome = turmaDto.Nome,
                Descricao = turmaDto.Descricao
            };

            await _turmaRepository.AddAsync(turma);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Turma turma = await _turmaRepository.GetByIdAsync(id);

            if (turma == null)
            {
                return false;
            }

            await _turmaRepository.DeleteAsync(turma.Id);

            return true;
        }

        public async Task<PagedResult<TurmaDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            PagedResult<Turma> pagedResultEntity = await _turmaRepository.GetAllAsync(pageNumber, pageSize);

            List<TurmaDto> itemsDto = pagedResultEntity.Items
                .Select(t => new TurmaDto(t.Id, t.Nome, t.Descricao))
                .ToList();

            return new PagedResult<TurmaDto>(
                itemsDto,
                pagedResultEntity.TotalCount,
                pagedResultEntity.PageNumber,
                pagedResultEntity.PageSize
            );
        }

        public async Task<TurmaDto> GetByIdAsync(int id)
        {
            Turma turma = await _turmaRepository.GetByIdAsync(id);

            if (turma == null)
            {
                return null;
            }

            return new TurmaDto(turma.Id, turma.Nome, turma.Descricao);
        }

        public async Task UpdateAsync(TurmaDto turmaDto)
        {
            Turma turma = await _turmaRepository.GetByIdAsync(turmaDto.Id);

            if (turma != null)
            {
                turma.Nome = turmaDto.Nome;
                turma.Descricao = turmaDto.Descricao;

                await _turmaRepository.UpdateAsync(turma);
            }
        }
    }
}
