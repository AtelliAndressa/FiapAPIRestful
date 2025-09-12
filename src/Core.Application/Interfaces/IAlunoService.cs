using Core.Application.DTOs;

namespace Core.Application.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoDto> GetByIdAsync(int id);

        Task<IEnumerable<AlunoDto>> GetAllAsync();

        Task AddAsync(AlunoDto alunoDto);

        Task UpdateAsync(AlunoDto alunoDto);

        Task<bool> DeleteAsync(int id);
    }
}
