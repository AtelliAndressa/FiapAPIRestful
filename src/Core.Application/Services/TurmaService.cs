using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Interfaces;
using FluentValidation;
using FluentValidation.Results;

namespace Core.Application.Services
{
    public class TurmaService : ITurmaService
    {
        private readonly ITurmaRepository _turmaRepository;
        private readonly IValidator<CreateTurmaDto> _createValidator;
        private readonly IValidator<TurmaDto> _validator;

        public TurmaService(ITurmaRepository turmaRepository,
            IValidator<CreateTurmaDto> createValidator,
            IValidator<TurmaDto> validator)
        {
            _turmaRepository = turmaRepository;
            _createValidator = createValidator;
            _validator = validator;
        }

        public async Task AddAsync(CreateTurmaDto turmaDto)
        {
            ValidationResult validationResult = await _createValidator.ValidateAsync(turmaDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            bool turmaExists = await _turmaRepository.IsTeamAsync(turmaDto.Nome);

            if (turmaExists)
            {
                throw new ValidationException("Já existe uma turma com este nome.");
            }

            Turma turma = new Turma
            {
                Nome = turmaDto.Nome,
                Descricao = turmaDto.Descricao
            };

            await _turmaRepository.AddAsync(turma);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            Turma turma = await _turmaRepository.GetByIdAsync(id);

            if (turma == null)
            {
                return false;
            }

            await _turmaRepository.DeleteAsync(turma.Id);

            return true;
        }

        public async Task<PagedResult<TurmaDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            PagedResult<Turma> pagedResultEntity = await _turmaRepository.GetAllAsync(pageNumber, pageSize);

            List<TurmaDto> itemsDto = pagedResultEntity.Items
                .Select(t => new TurmaDto(
                    t.Id,
                    t.Nome,
                    t.Descricao,
                    t.Matriculas.Count()
                ))
                .ToList();

            return new PagedResult<TurmaDto>(
                itemsDto,
                pagedResultEntity.TotalCount,
                pagedResultEntity.PageNumber,
                pagedResultEntity.PageSize
            );
        }

        public async Task<TurmaDto> GetByIdAsync(int id)
        {
            Turma turma = await _turmaRepository.GetByIdAsync(id);

            if (turma == null)
            {
                throw new ValidationException("Turma não encontrada.");
            }

            return new TurmaDto(turma.Id, turma.Nome, turma.Descricao, 0);
        }

        public async Task UpdateAsync(TurmaDto turmaDto)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(turmaDto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            Turma turma = await _turmaRepository.GetByIdAsync(turmaDto.Id);

            if (turma == null)
            {
                throw new ValidationException("Turma não encontrada.");
            }

            turma.Nome = turmaDto.Nome;
            turma.Descricao = turmaDto.Descricao;

            await _turmaRepository.UpdateAsync(turma);
        }
    }
}
