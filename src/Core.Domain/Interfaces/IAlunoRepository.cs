using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> GetByIdAsync(int id);

        Task<PagedResult<Aluno>> GetAllAsync(int pageNumber, int pageSize);

        Task AddAsync(Aluno aluno);

        Task UpdateAsync(Aluno aluno);

        Task DeleteAsync(int id);

        Task<Aluno> GetByCpfAsync(string cpf);
    }
}
