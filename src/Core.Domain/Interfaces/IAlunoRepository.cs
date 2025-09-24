using Core.Domain.Common;
using Core.Domain.Entities;

namespace Core.Domain.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> GetByIdAsync(Guid id);

        Task<PagedResult<Aluno>> GetAllAsync(int pageNumber, int pageSize);

        Task<PagedResult<Aluno>> SearchByNameAsync(string nome, int pageNumber, int pageSize);

        Task<Aluno> GetByEmailAsync(string email);

        Task AddAsync(Aluno aluno);

        Task UpdateAsync(Aluno aluno);

        Task DeleteAsync(Guid id);

        Task<Aluno> GetByCpfAsync(string cpf);
    }
}
