using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Repositories
{
    public class TurmaRepository : ITurmaRepository
    {
        private readonly ApplicationDbContext _context;

        public TurmaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Turma>> GetAllAsync(int pageNumber, int pageSize)
        {
            int totalCount = await _context.Turmas.CountAsync();

            List<Turma> items = await _context.Turmas
                .OrderBy(t => t.Nome)
                .Include(t => t.Matriculas)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Turma>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<Turma> GetByIdAsync(Guid id)
        {
            Turma turma = await _context.Turmas.FirstOrDefaultAsync(c => c.Id == id);

            if (turma == null)
            {
                throw new ValidationException("Turma não encontrada.");
            }

            return turma;
        }

        public async Task AddAsync(Turma turma)
        {
            await _context.Turmas.AddAsync(turma);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTeamAsync(string nome)
        {
            return await _context.Turmas
                .AnyAsync(m => m.Nome == nome);
        }

        public async Task UpdateAsync(Turma turma)
        {
            Turma turmaEditada = await _context.Turmas.FindAsync(turma.Id);

            if (turmaEditada == null)
            {
                throw new ValidationException("Turma não encontrada.");
            }

            turmaEditada.Nome = turma.Nome;
            turmaEditada.Descricao = turma.Descricao;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            Turma turma = await _context.Turmas.FindAsync(id);

            if (turma != null)
            {
                _context.Turmas.Remove(turma);

                await _context.SaveChangesAsync();
            }
        }
    }
}
