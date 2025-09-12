using Core.Application.DTOs;

namespace Core.Application.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoDto> GetByIdAsync(int id);

        Task<IEnumerable<AlunoDto>> GetAllAsync();

        Task<AlunoDto> AddAsync(AlunoDto alunoDto);

        Task<AlunoDto> UpdateAsync(AlunoDto alunoDto);

        Task<bool> DeleteAsync(int id);
    }
}
