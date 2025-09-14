using Core.Application.DTOs;
using FluentValidation;

namespace Core.Application.Validators;

public class MatriculaDtoValidator : AbstractValidator<MatriculaDto>
{
    public MatriculaDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id da matricula é obrigatório.");

        RuleFor(x => x.Aluno)
            .NotNull().WithMessage("O aluno é obrigatório.");

        RuleFor(x => x.Turma)
            .NotNull().WithMessage("A turma é obrigatória.");

        RuleFor(x => x.DataMatricula)
            .NotEmpty().WithMessage("A data de matrícula é obrigatória.")
            .Must(data => data <= DateTime.Now)
            .WithMessage("A data de matrícula não pode ser no futuro.");
    }
}