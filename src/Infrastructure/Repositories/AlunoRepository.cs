using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly ApplicationDbContext _context;

        public AlunoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Aluno aluno)
        {
            await _context.Alunos.AddAsync(aluno);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Aluno aluno)
        {
            Aluno alunoEditado = await _context.Alunos.FindAsync(aluno.Id);

            if (alunoEditado != null)
            {
                alunoEditado.Nome = aluno.Nome;
                alunoEditado.Email = aluno.Email;
                alunoEditado.Cpf = aluno.Cpf;
                alunoEditado.DataNascimento = aluno.DataNascimento;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            Aluno aluno = await _context.Alunos.FindAsync(id);

            if (aluno != null)
            {
                _context.Alunos.Remove(aluno);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PagedResult<Aluno>> GetAllAsync(int pageNumber, int pageSize)
        {
            int totalCount = await _context.Alunos.CountAsync();

            List<Aluno> items = await _context.Alunos
                .OrderBy(a => a.Nome)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<Aluno>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<Aluno> GetByIdAsync(int id)
        {
            Aluno aluno = await _context.Alunos
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno == null)
            {
                throw new Exception("Aluno não encontrado");
            }
            
            return aluno;
        }
    }
}
