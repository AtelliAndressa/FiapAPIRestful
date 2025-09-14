using Core.Application.DTOs;
using FluentValidation;

namespace Core.Application.Validators;

public class CreateMatriculaDtoValidator : AbstractValidator<CreateMatriculaDto>
{
    public CreateMatriculaDtoValidator()
    {
        RuleFor(x => x.AlunoId)
            .NotEmpty().WithMessage("O AlunoId é obrigatório.");

        RuleFor(x => x.TurmaId)
            .NotEmpty().WithMessage("O TurmaId é obrigatório.");

        RuleFor(x => x.DataMatricula)
            .NotEmpty().WithMessage("A data de matrícula é obrigatória.")
            .Must(data => data <= DateTime.Now)
            .WithMessage("A data de matrícula não pode ser no futuro.");
    }
}
