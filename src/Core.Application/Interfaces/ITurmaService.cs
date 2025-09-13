using Core.Application.DTOs;

namespace Core.Application.Interfaces
{
    public interface ITurmaService
    {
        Task<TurmaDto> GetByIdAsync(int id);

        Task<IEnumerable<TurmaDto>> GetAllAsync();

        Task AddAsync(TurmaDto TurmaDto);

        Task UpdateAsync(TurmaDto TurmaDto);

        Task<bool> DeleteAsync(int id);
    }
}
