using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface IMatriculaRepository
    {
        Task<Matricula> GetByIdAsync(Guid id);

        Task<PagedResult<Matricula>> GetAllAsync(int pageNumber, int pageSize);

        Task AddAsync(Matricula matricula);

        Task UpdateAsync(Matricula matricula);

        Task DeleteAsync(Guid id);

        Task<PagedResult<Matricula>> GetByStudentIdAsync(Guid alunoId, int pageNumber, int pageSize);

        Task<PagedResult<Matricula>> GetByTeamIdAsync(Guid turmaId, int pageNumber, int pageSize);

        Task<bool> IsStudentAlreadyEnrolledAsync(Guid alunoId, Guid turmaId);
    }
}
