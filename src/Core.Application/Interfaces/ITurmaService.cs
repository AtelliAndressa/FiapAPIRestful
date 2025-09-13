using Core.Application.DTOs;
using Core.Domain.Common;

namespace Core.Application.Interfaces
{
    public interface ITurmaService
    {
        Task<TurmaDto> GetByIdAsync(int id);

        Task<PagedResult<TurmaDto>> GetAllAsync(int pageNumber, int pageSize);

        Task AddAsync(CreateTurmaDto turmaDto);

        Task UpdateAsync(int id, UpdateTurmaDto turmaDto);

        Task<bool> DeleteAsync(int id);
    }
}
