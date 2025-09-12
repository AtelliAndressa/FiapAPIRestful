using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Interfaces
{
    public interface IMatriculaRepository
    {
        Task<Matricula> GetByIdAsync(int id);

        Task<IEnumerable<Matricula>> GetAllAsync();

        Task AddAsync(Matricula matricula);

        Task UpdateAsync(Matricula matricula);

        Task DeleteAsync(int id);

        Task<IEnumerable<Matricula>> GetByAlunoIdAsync(int alunoId);

        Task<IEnumerable<Matricula>> GetByCourseIdAsync(int cursoId);
    }
}
