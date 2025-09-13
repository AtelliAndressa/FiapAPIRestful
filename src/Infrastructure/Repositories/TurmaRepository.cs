using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Turma>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<Turma> GetByIdAsync(int id)
        {
            Turma turma = await _context.Turmas.FirstOrDefaultAsync(c => c.Id == id);

            if (turma == null)
            {
                throw new KeyNotFoundException("Turma not found");
            }

            return turma;
        }

        public async Task AddAsync(Turma turma)
        {
            await _context.Turmas.AddAsync(turma);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Turma turma)
        {
            Turma turmaEditado = await _context.Turmas.FindAsync(turma.Id);
            
            if (turmaEditado != null)
            {
                turmaEditado.Nome = turma.Nome;
                turmaEditado.Descricao = turma.Descricao;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
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
