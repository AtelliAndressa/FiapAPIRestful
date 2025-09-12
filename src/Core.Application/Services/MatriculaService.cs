using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Interfaces;

namespace Core.Application.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _matriculaRepository;

        public MatriculaService(IMatriculaRepository matriculaRepository)
        {
            _matriculaRepository = matriculaRepository;
        }

        public async Task AddAsync(MatriculaDto matricula)
        {
            Matricula newMatricula = new Matricula
            {
                AlunoId = matricula.AlunoId,
                CursoId = matricula.CursoId,
                DataMatricula = matricula.DataMatricula
            };

            await _matriculaRepository.AddAsync(newMatricula);
        }

        public async Task UpdateAsync(MatriculaDto matricula)
        {
            var existingMatricula = await _matriculaRepository.GetByIdAsync(matricula.Id);

            if (existingMatricula == null)
            {
                return;
            }

            existingMatricula.AlunoId = matricula.AlunoId;
            existingMatricula.CursoId = matricula.CursoId;
            existingMatricula.DataMatricula = matricula.DataMatricula;
                
            await _matriculaRepository.UpdateAsync(existingMatricula);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Matricula matricula = await _matriculaRepository.GetByIdAsync(id);

            if (matricula == null)
            {
                return false;
            }

            await _matriculaRepository.DeleteAsync(matricula.Id);

            return true;
        }

        public async Task<IEnumerable<MatriculaDto>> GetAllAsync()
        {
            IEnumerable<Matricula> matriculas = await _matriculaRepository.GetAllAsync();

            return matriculas.Select(m => new MatriculaDto(m.Id, m.AlunoId, m.CursoId, m.DataMatricula));
        }

        public async Task<MatriculaDto> GetByIdAsync(int id)
        {
            Matricula matricula = await _matriculaRepository.GetByIdAsync(id);

            if (matricula == null)
            {
                return null;
            }

            return new MatriculaDto(matricula.Id, matricula.AlunoId, matricula.CursoId, matricula.DataMatricula);
        }

        public async Task<IEnumerable<MatriculaDto>> GetByStudentIdAsync(int studentId)
        {
            IEnumerable<Matricula> matriculas = await _matriculaRepository.GetByAlunoIdAsync(studentId);

            return matriculas.Select(m => new MatriculaDto(m.Id, m.AlunoId, m.CursoId, m.DataMatricula));
        }

        public async Task<IEnumerable<MatriculaDto>> GetByCourseIdAsync(int courseId)
        {
            IEnumerable<Matricula> matriculas = await _matriculaRepository.GetByCourseIdAsync(courseId);

            return matriculas.Select(m => new MatriculaDto(m.Id, m.AlunoId, m.CursoId, m.DataMatricula));
        }
    }
}
