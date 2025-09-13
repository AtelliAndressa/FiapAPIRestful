using Core.Application.DTOs;
using Core.Domain.Entities;

namespace Core.Application.Interfaces
{
    public interface IMatriculaService
    {
        Task<MatriculaDto> GetByIdAsync(int id);

        Task<IEnumerable<MatriculaDto>> GetAllAsync();

        Task AddAsync(MatriculaDto matricula);

        Task UpdateAsync(MatriculaDto matricula);

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<MatriculaDto>> GetByStudentIdAsync(int studentId);

        Task<IEnumerable<MatriculaDto>> GetByTeamIdAsync(int courseId);
    }
}
