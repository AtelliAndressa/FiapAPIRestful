using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface ITurmaRepository
    {
        Task<Turma> GetByIdAsync(int id);

        Task<PagedResult<Turma>> GetAllAsync(int pageNumber, int pageSize);

        Task AddAsync(Turma turma);

        Task UpdateAsync(Turma turma);

        Task DeleteAsync(int id);

        Task<bool> IsTeamAsync(string nome);
    }
}
