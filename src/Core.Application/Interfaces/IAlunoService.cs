using Core.Application.DTOs;
using Core.Domain.Common;

namespace Core.Application.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoDto> GetByIdAsync(int id);

        Task<PagedResult<AlunoDto>> GetAllAsync(int pageNumber, int pageSize);

        Task AddAsync(AlunoDto alunoDto);

        Task UpdateAsync(AlunoDto alunoDto);

        Task<bool> DeleteAsync(int id);
    }
}
