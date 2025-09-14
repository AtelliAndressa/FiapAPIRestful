using Core.Application.DTOs;
using Core.Domain.Common;

namespace Core.Application.Interfaces
{
    public interface ITurmaService
    {
        Task<TurmaDto> GetByIdAsync(Guid id);

        Task<PagedResult<TurmaDto>> GetAllAsync(int pageNumber, int pageSize);

        Task AddAsync(CreateTurmaDto turmaDto);

        Task UpdateAsync(Guid id, UpdateTurmaDto turmaDto);

        Task<bool> DeleteAsync(Guid id);
    }
}
