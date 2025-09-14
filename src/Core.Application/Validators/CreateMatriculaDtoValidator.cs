using Core.Application.DTOs;
using FluentValidation;

namespace Core.Application.Validators;

public class CreateMatriculaDtoValidator : AbstractValidator<CreateMatriculaDto>
{
    public CreateMatriculaDtoValidator()
    {
        RuleFor(x => x.AlunoId)
            .NotEmpty().WithMessage("O AlunoId é obrigatório e não pode ser nulo.");

        RuleFor(x => x.TurmaId)
            .NotEmpty().WithMessage("O TurmaId é obrigatório e não pode ser nulo..");

        RuleFor(x => x.DataMatricula)
            .NotEmpty().WithMessage("A data de matrícula é obrigatória e não pode ser nula.")
            .Must(data => data <= DateTime.Now)
            .WithMessage("A data de matrícula não pode ser no futuro.");
    }
}
