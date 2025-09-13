using Core.Application.DTOs;
using Core.Domain.Common;

namespace Core.Application.Interfaces
{
    public interface IMatriculaService
    {
        Task<MatriculaDto> GetByIdAsync(int id);

        Task<MatriculaDto> AddAsync(CreateMatriculaDto matricula);

        Task UpdateAsync(MatriculaDto matricula);

        Task<bool> DeleteAsync(int id);

        Task<PagedResult<MatriculaDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<PagedResult<MatriculaDto>> GetByStudentIdAsync(int alunoId, int pageNumber, int pageSize);

        Task<PagedResult<MatriculaDto>> GetByTeamIdAsync(int turmaId, int pageNumber, int pageSize);
    }
}
