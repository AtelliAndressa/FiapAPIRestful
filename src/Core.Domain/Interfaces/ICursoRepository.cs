using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Interfaces
{
    public interface ICursoRepository
    {
        Task<Curso> GetByIdAsync(int id);

        Task<IEnumerable<Curso>> GetAllAsync();

        Task AddAsync(Curso curso);

        Task UpdateAsync(Curso curso);

        Task DeleteAsync(int id);
    }
}
