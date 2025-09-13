using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Core.Application.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly IAlunoRepository _alunoRepository;
        private readonly ITurmaRepository _turmaRepository;

        public MatriculaService(
            IMatriculaRepository matriculaRepository,
            IAlunoRepository alunoRepository,
            ITurmaRepository turmaRepository)
        {
            _matriculaRepository = matriculaRepository;
            _alunoRepository = alunoRepository;
            _turmaRepository = turmaRepository;
        }

        public async Task<MatriculaDto> AddAsync(CreateMatriculaDto matriculaDto)
        {
            Aluno aluno = await _alunoRepository.GetByIdAsync(matriculaDto.AlunoId);

            if (aluno == null)
            {
                throw new ValidationException("Aluno não encontrado. Por favor cadastre o Aluno antes de efetuar a matrícula.");
            }

            Turma turma = await _turmaRepository.GetByIdAsync(matriculaDto.TurmaId);

            if (turma == null)
            {
                throw new ValidationException("Turma não encontrada. Por favor cadastre a turma antes de efetuar a matrícula.");
            }

            var jaMatriculado = await _matriculaRepository.IsStudentAlreadyEnrolledAsync(matriculaDto.AlunoId, matriculaDto.TurmaId);

            if (jaMatriculado)
            {
                throw new ValidationException("O aluno já está matriculado nesta turma.");
            }

            Matricula novaMatricula = new Matricula
            {
                AlunoId = matriculaDto.AlunoId,
                TurmaId = matriculaDto.TurmaId,
                DataMatricula = matriculaDto.DataMatricula
            };

            await _matriculaRepository.AddAsync(novaMatricula);

            return new MatriculaDto(
                novaMatricula.Id,
                new AlunoDto(aluno.Id, aluno.Nome, aluno.Cpf, aluno.Email, aluno.DataNascimento),
                new TurmaDto(turma.Id, turma.Nome, turma.Descricao),
                novaMatricula.DataMatricula
            );
        }

        public async Task UpdateAsync(MatriculaDto matricula)
        {
            var existingMatricula = await _matriculaRepository.GetByIdAsync(matricula.Id);

            if (existingMatricula == null)
            {
                return;
            }

            existingMatricula.AlunoId = matricula.AlunoId;
            existingMatricula.TurmaId = matricula.TurmaId;
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

            return matriculas.Select(m => new MatriculaDto(
                m.Id,
                new AlunoDto(m.AlunoId, m.Aluno.Nome, m.Aluno.Cpf, m.Aluno.Email, m.Aluno.DataNascimento),
                new TurmaDto(m.Turma.Id, m.Turma.Nome, m.Turma.Descricao),
                m.DataMatricula
            ));
        }

        public async Task<MatriculaDto> GetByIdAsync(int id)
        {
            Matricula matricula = await _matriculaRepository.GetByIdAsync(id);

            if (matricula == null)
            {
                return null;
            }

            return new MatriculaDto(matricula.Id, matricula.AlunoId, matricula.TurmaId, matricula.DataMatricula);
        }

        public async Task<IEnumerable<MatriculaDto>> GetByStudentIdAsync(int studentId)
        {
            IEnumerable<Matricula> matriculas = await _matriculaRepository.GetByAlunoIdAsync(studentId);

            return matriculas.Select(m => new MatriculaDto(m.Id, m.AlunoId, m.TurmaId, m.DataMatricula));
        }

        public async Task<IEnumerable<MatriculaDto>> GetByTeamIdAsync(int courseId)
        {
            IEnumerable<Matricula> matriculas = await _matriculaRepository.GetByTeamIdAsync(courseId);

            return matriculas.Select(m => new MatriculaDto(m.Id, m.AlunoId, m.TurmaId, m.DataMatricula));
        }
    }
}
