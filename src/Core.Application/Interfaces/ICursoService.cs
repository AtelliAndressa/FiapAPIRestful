using Core.Application.DTOs;

namespace Core.Application.Interfaces
{
    public interface ICursoService
    {
        Task<CursoDto> GetByIdAsync(int id);

        Task<IEnumerable<CursoDto>> GetAllAsync();

        Task AddAsync(CursoDto cursoDto);

        Task UpdateAsync(CursoDto cursoDto);

        Task<bool> DeleteAsync(int id);
    }
}
