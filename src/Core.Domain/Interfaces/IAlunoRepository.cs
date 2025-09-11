using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> GetByIdAsync(int id);

        Task<IEnumerable<Aluno>> GetAllAsync();

        Task AddAsync(Aluno aluno);

        Task UpdateAsync(Aluno aluno);

        Task DeleteAsync(int id);
    }
}
