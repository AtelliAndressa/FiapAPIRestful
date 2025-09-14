using Core.Application.DTOs;
using FluentValidation;

namespace Core.Application.Validators;

public class MatriculaDtoValidator : AbstractValidator<MatriculaDto>
{
    public MatriculaDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id da matricula é obrigatório e não pode ser nulo..");

        RuleFor(x => x.Aluno)
            .NotNull().WithMessage("O aluno é obrigatório e não pode ser nulo.");

        RuleFor(x => x.Turma)
            .NotNull().WithMessage("A turma é obrigatória e não pode ser nula.");

        RuleFor(x => x.DataMatricula)
            .NotEmpty().WithMessage("A data de matrícula é obrigatória e não pode ser nula.")
            .Must(data => data <= DateTime.Now)
            .WithMessage("A data de matrícula não pode ser no futuro.");
    }
}