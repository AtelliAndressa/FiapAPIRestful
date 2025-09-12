using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Services
{
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _cursoRepository;

        public CursoService(ICursoRepository cursoRepository)
        {
            _cursoRepository = cursoRepository;
        }

        public async Task AddAsync(CursoDto cursoDto)
        {
            Curso curso = new Core.Domain.Entities.Curso
            {
                Nome = cursoDto.Nome,
                Descricao = cursoDto.Descricao
            };

            await _cursoRepository.AddAsync(curso);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var curso = await _cursoRepository.GetByIdAsync(id);

            if (curso == null)
            {
                return false;
            }

            await _cursoRepository.DeleteAsync(curso.Id);

            return true;
        }

        public async Task<IEnumerable<CursoDto>> GetAllAsync()
        {
            IEnumerable<Curso> cursos = await _cursoRepository.GetAllAsync();

            return cursos.Select(c => new CursoDto(c.Id, c.Nome, c.Descricao));
        }

        public async Task<CursoDto> GetByIdAsync(int id)
        {
            Curso curso = await _cursoRepository.GetByIdAsync(id);

            if (curso == null)
            {
                return null;
            }

            return new CursoDto(curso.Id, curso.Nome, curso.Descricao);
        }

        public async Task UpdateAsync(CursoDto cursoDto)
        {
            var curso = await _cursoRepository.GetByIdAsync(cursoDto.Id);

            if (curso != null)
            {
                curso.Nome = cursoDto.Nome;
                curso.Descricao = cursoDto.Descricao;
                await _cursoRepository.UpdateAsync(curso);
            }
        }
    }
}
