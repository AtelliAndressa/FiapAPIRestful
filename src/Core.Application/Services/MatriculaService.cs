using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Application.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly IMatriculaRepository _matriculaRepository;
        private readonly IAlunoRepository _alunoRepository;
        private readonly ITurmaRepository _turmaRepository;
        private readonly IValidator<CreateMatriculaDto> _createValidator;
        private readonly IValidator<MatriculaDto> _validator;

        public MatriculaService(
            IMatriculaRepository matriculaRepository,
            IAlunoRepository alunoRepository,
            ITurmaRepository turmaRepository,
            IValidator<CreateMatriculaDto> createValidator,
            IValidator<MatriculaDto> validator)
        {
            _matriculaRepository = matriculaRepository;
            _alunoRepository = alunoRepository;
            _turmaRepository = turmaRepository;
            _createValidator = createValidator;
            _validator = validator;
        }

        public async Task<MatriculaDto> AddAsync(CreateMatriculaDto matriculaDto)
        {
            ValidationResult validationResult = await _createValidator.ValidateAsync(matriculaDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

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

            if (matriculaDto.DataMatricula == default)
            {
                throw new ValidationException("Data da matrícula é obrigatória.");
            }

            bool jaMatriculado = await _matriculaRepository.IsStudentAlreadyEnrolledAsync(matriculaDto.AlunoId, matriculaDto.TurmaId);

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
            ValidationResult validationResult = await _validator.ValidateAsync(matricula);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Matricula existingMatricula = await _matriculaRepository.GetByIdAsync(matricula.Id);

            if (existingMatricula == null)
            {
                throw new ValidationException("Matrícula não encontrada.");
            }

            existingMatricula.AlunoId = matricula.Aluno.Id;
            existingMatricula.TurmaId = matricula.Turma.Id;
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

        public async Task<MatriculaDto> GetByIdAsync(int id)
        {
            Matricula matricula = await _matriculaRepository.GetByIdAsync(id);

            if (matricula == null)
            {
                throw new ValidationException("Matrícula não encontrada.");
            }

            return new MatriculaDto(
                matricula.Id,
                new AlunoDto(matricula.Aluno.Id, matricula.Aluno.Nome, matricula.Aluno.Cpf, matricula.Aluno.Email, matricula.Aluno.DataNascimento),
                new TurmaDto(matricula.Turma.Id, matricula.Turma.Nome, matricula.Turma.Descricao),
                matricula.DataMatricula
            );
        }

        private List<MatriculaDto> MapToDto(List<Matricula> matriculas)
        {
            return matriculas.Select(m => new MatriculaDto(
                m.Id,
                new AlunoDto(m.Aluno.Id, m.Aluno.Nome, m.Aluno.Cpf, m.Aluno.Email, m.Aluno.DataNascimento),
                new TurmaDto(m.Turma.Id, m.Turma.Nome, m.Turma.Descricao),
                m.DataMatricula
            )).ToList();
        }

        public async Task<PagedResult<MatriculaDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            PagedResult<Matricula> pagedResult = await _matriculaRepository.GetAllAsync(pageNumber, pageSize);

            List<MatriculaDto> itemsDto = MapToDto(pagedResult.Items);

            return new PagedResult<MatriculaDto>(itemsDto, pagedResult.TotalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<MatriculaDto>> GetByStudentIdAsync(int alunoId, int pageNumber, int pageSize)
        {
            PagedResult<Matricula> pagedResult = await _matriculaRepository.GetByStudentIdAsync(alunoId, pageNumber, pageSize);

            List<MatriculaDto> itemsDto = MapToDto(pagedResult.Items);

            return new PagedResult<MatriculaDto>(itemsDto, pagedResult.TotalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<MatriculaDto>> GetByTeamIdAsync(int turmaId, int pageNumber, int pageSize)
        {
            PagedResult<Matricula> pagedResult = await _matriculaRepository.GetByTeamIdAsync(turmaId, pageNumber, pageSize);

            List<MatriculaDto> itemsDto = MapToDto(pagedResult.Items);

            return new PagedResult<MatriculaDto>(itemsDto, pagedResult.TotalCount, pageNumber, pageSize);
        }
    }
}
