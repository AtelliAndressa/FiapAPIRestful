using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface IMatriculaRepository
    {
        Task<Matricula> GetByIdAsync(int id);

        Task<PagedResult<Matricula>> GetAllAsync(int pageNumber, int pageSize);

        Task AddAsync(Matricula matricula);

        Task UpdateAsync(Matricula matricula);

        Task DeleteAsync(int id);

        Task<PagedResult<Matricula>> GetByStudentIdAsync(int alunoId, int pageNumber, int pageSize);

        Task<PagedResult<Matricula>> GetByTeamIdAsync(int turmaId, int pageNumber, int pageSize);

        Task<bool> IsStudentAlreadyEnrolledAsync(int alunoId, int turmaId);
    }
}
