using Core.Domain.Common;
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

        public async Task<Matricula> GetByIdAsync(Guid id)
        {
            Matricula matricula = await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (matricula == null)
            {
                return null;
            }

            return matricula;
        }

        public async Task AddAsync(Matricula matricula)
        {
            await _context.Matriculas.AddAsync(matricula);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Matricula matricula)
        {
            Matricula existingMatricula = await _context.Matriculas
                .FirstOrDefaultAsync(m => m.Id == matricula.Id);

            if (existingMatricula == null)
            {
                throw new KeyNotFoundException("Matrícula não encontrada.");
            }

            existingMatricula.AlunoId = matricula.AlunoId;
            existingMatricula.TurmaId = matricula.TurmaId;
            existingMatricula.DataMatricula = matricula.DataMatricula;
        }

        public async Task<bool> IsStudentAlreadyEnrolledAsync(Guid alunoId, Guid turmaId)
        {
            return await _context.Matriculas
                .AnyAsync(m => m.AlunoId == alunoId && m.TurmaId == turmaId);
        }

        public async Task DeleteAsync(Guid id)
        {
            Matricula matricula = await _context.Matriculas.FindAsync(id);

            if (matricula != null)
            {
                _context.Matriculas.Remove(matricula);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<Matricula>> GetAllAsync(int pageNumber, int pageSize)
        {
            int totalCount = await _context.Matriculas.CountAsync();

            List<Matricula> items = await _context.Matriculas
                .Include(m => m.Aluno)
                .Include(m => m.Turma)
                .OrderBy(m => m.Aluno.Nome)
                .ThenBy(m => m.Turma.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Matricula>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<Matricula>> GetByStudentIdAsync(Guid alunoId, int pageNumber, int pageSize)
        {
            IQueryable<Matricula> query = _context.Matriculas
                .Where(m => m.AlunoId == alunoId);

            int totalCount = await query.CountAsync();

            List<Matricula> items = await query
                .Include(m => m.Aluno)
                .Include(m => m.Turma)
                .OrderBy(m => m.Turma.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Matricula>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<Matricula>> GetByTeamIdAsync(Guid turmaId, int pageNumber, int pageSize)
        {
            IQueryable<Matricula> query = _context.Matriculas
                .Where(m => m.TurmaId == turmaId);

            int totalCount = await query.CountAsync();

            List<Matricula> items = await query
                .Include(m => m.Aluno)
                .Include(m => m.Turma)
                .OrderBy(m => m.Aluno.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Matricula>(items, totalCount, pageNumber, pageSize);
        }
    }
}
