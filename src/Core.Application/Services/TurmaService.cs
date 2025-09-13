using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Services
{
    public class TurmaService : ITurmaService
    {
        private readonly ITurmaRepository _TurmaRepository;

        public TurmaService(ITurmaRepository TurmaRepository)
        {
            _TurmaRepository = TurmaRepository;
        }

        public async Task AddAsync(TurmaDto turmaDto)
        {
            Turma turma = new Turma
            {
                Nome = turmaDto.Nome,
                Descricao = turmaDto.Descricao
            };

            await _TurmaRepository.AddAsync(turma);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var turma = await _TurmaRepository.GetByIdAsync(id);

            if (turma == null)
            {
                return false;
            }

            await _TurmaRepository.DeleteAsync(turma.Id);

            return true;
        }

        public async Task<IEnumerable<TurmaDto>> GetAllAsync()
        {
            IEnumerable<Turma> turmas = await _TurmaRepository.GetAllAsync();

            return turmas.Select(c => new TurmaDto(c.Id, c.Nome, c.Descricao));
        }

        public async Task<TurmaDto> GetByIdAsync(int id)
        {
            Turma turma = await _TurmaRepository.GetByIdAsync(id);

            if (turma == null)
            {
                return null;
            }

            return new TurmaDto(turma.Id, turma.Nome, turma.Descricao);
        }

        public async Task UpdateAsync(TurmaDto turmaDto)
        {
            var turma = await _TurmaRepository.GetByIdAsync(turmaDto.Id);

            if (turma != null)
            {
                turma.Nome = turmaDto.Nome;
                turma.Descricao = turmaDto.Descricao;

                await _TurmaRepository.UpdateAsync(turma);
            }
        }
    }
}
