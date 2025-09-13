using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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

        public async Task<Aluno> GetByCpfAsync(string cpf)
        {
            return await _context.Alunos
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Cpf == cpf);
        }

        public async Task UpdateAsync(Aluno aluno)
        {
            Aluno alunoEditado = await _context.Alunos.FirstOrDefaultAsync(a => a.Id == aluno.Id);

            if (alunoEditado == null)
            {
                throw new ValidationException("Aluno não encontrado.");
            }

            alunoEditado.Nome = aluno.Nome;
            alunoEditado.Email = aluno.Email;
            alunoEditado.Cpf = aluno.Cpf;
            alunoEditado.DataNascimento = aluno.DataNascimento;

            await _context.SaveChangesAsync();
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

        public async Task<PagedResult<Aluno>> SearchByNameAsync(string nome, int pageNumber, int pageSize)
        {
            IQueryable<Aluno> query = _context.Alunos
                .Where(a => a.Nome.ToLower().Contains(nome.ToLower()));

            int totalCount = await query.CountAsync();

            var items = await query
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
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno == null)
            {
                throw new ValidationException("Aluno não encontrado.");
            }
            
            return aluno;
        }
    }
}
