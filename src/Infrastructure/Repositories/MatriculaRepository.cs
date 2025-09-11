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
    public class MatriculaRepository : IMatriculaRepository
    {
        private readonly ApplicationDbContext _context;

        public MatriculaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Matricula matricula)
        {
            await _context.Matriculas.AddAsync(matricula);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);

            if (matricula != null)
            {
                _context.Matriculas.Remove(matricula);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Matricula>> GetAllAsync()
        {
            return await _context.Matriculas.ToListAsync();
        }

        public async Task<Matricula> GetByIdAsync(int id)
        {
            var matricula = await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.Id == id);

            return matricula;
        }

        public async Task UpdateAsync(Matricula matricula)
        {
            var matriculaEditada = await _context.Matriculas.FindAsync(matricula.Id);

            if (matriculaEditada != null)
            {
                matriculaEditada.AlunoId = matricula.AlunoId;
                matriculaEditada.CursoId = matricula.CursoId;
                matriculaEditada.DataMatricula = matricula.DataMatricula;

                await _context.SaveChangesAsync();
            }
        }
    }
}
