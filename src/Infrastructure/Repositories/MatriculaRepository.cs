using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
                .Include(m => m.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);

            return matricula;
        }

        public async Task UpdateAsync(Matricula matricula)
        {
            var matriculaEditada = await _context.Matriculas.FindAsync(matricula.Id);

            if (matriculaEditada != null)
            {
                matriculaEditada.AlunoId = matricula.AlunoId;
                matriculaEditada.TurmaId = matricula.TurmaId;
                matriculaEditada.DataMatricula = matricula.DataMatricula;

                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Matricula>> GetByAlunoIdAsync(int alunoId)
        {
            return await _context.Matriculas
                .Where(m => m.AlunoId == alunoId)
                .Include(m => m.Aluno)
                .Include(m => m.Turma)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Matricula>> GetByTeamIdAsync(int turmaId)
        {
            return await _context.Matriculas
                .Where(m => m.TurmaId == turmaId)
                .Include(m => m.Aluno)
                .Include(m => m.Turma)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> IsStudentAlreadyEnrolledAsync(int alunoId, int turmaId)
        {
            return await _context.Matriculas
                .AnyAsync(m => m.AlunoId == alunoId && m.TurmaId == turmaId);
        }
    }
}
