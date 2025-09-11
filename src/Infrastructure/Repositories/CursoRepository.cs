using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CursoRepository : ICursoRepository
    {
        private readonly ApplicationDbContext _context;

        public CursoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Curso curso)
        {
            await _context.Cursos.AddAsync(curso);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);

            if (curso != null)
            {
                _context.Cursos.Remove(curso);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Curso>> GetAllAsync()
        {
            return await _context.Cursos.ToListAsync();
        }

        public async Task<Curso> GetByIdAsync(int id)
        {
            var curso = await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);

            return curso;
        }

        public async Task UpdateAsync(Curso curso)
        {
            var cursoEditado = await _context.Cursos.FindAsync(curso.Id);
            
            if (cursoEditado != null)
            {
                cursoEditado.Nome = curso.Nome;
                cursoEditado.Descricao = curso.Descricao;
                await _context.SaveChangesAsync();
            }
        }
    }
}
