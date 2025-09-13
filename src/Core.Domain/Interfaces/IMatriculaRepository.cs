using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface IMatriculaRepository
    {
        Task<Matricula> GetByIdAsync(int id);

        Task<IEnumerable<Matricula>> GetAllAsync();

        Task AddAsync(Matricula matricula);

        Task UpdateAsync(Matricula matricula);

        Task DeleteAsync(int id);

        Task<IEnumerable<Matricula>> GetByAlunoIdAsync(int alunoId);

        Task<IEnumerable<Matricula>> GetByTeamIdAsync(int turmaId);

        Task<bool> IsStudentAlreadyEnrolledAsync(int alunoId, int turmaId);
    }
}
