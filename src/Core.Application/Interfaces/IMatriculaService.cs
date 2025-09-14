using Core.Application.DTOs;
using Core.Domain.Common;

namespace Core.Application.Interfaces
{
    public interface IMatriculaService
    {
        Task<MatriculaDto> GetByIdAsync(Guid id);

        Task<MatriculaDto> AddAsync(CreateMatriculaDto matricula);

        Task UpdateAsync(MatriculaDto matricula);

        Task<bool> DeleteAsync(Guid id);

        Task<PagedResult<MatriculaDto>> GetAllAsync(int pageNumber, int pageSize);

        Task<PagedResult<MatriculaDto>> GetByStudentIdAsync(Guid alunoId, int pageNumber, int pageSize);

        Task<PagedResult<MatriculaDto>> GetByTeamIdAsync(Guid turmaId, int pageNumber, int pageSize);
    }
}
