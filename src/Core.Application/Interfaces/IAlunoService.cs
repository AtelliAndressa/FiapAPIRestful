using Core.Application.DTOs;
using Core.Domain.Common;

namespace Core.Application.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoDto> GetByIdAsync(Guid id);

        Task<PagedResult<AlunoDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<PagedResult<AlunoDto>> SearchByNameAsync(string nome, int pageNumber, int pageSize);

        Task AddAsync(CreateAlunoDto alunoDto);

        Task UpdateAsync(Guid id, UpdateAlunoDto alunoDto);

        Task<bool> DeleteAsync(Guid id);
    }
}
