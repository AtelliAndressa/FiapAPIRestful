using Core.Application.DTOs;
using FluentValidation;

namespace Core.Application.Validators;

public class UpdateTurmaDtoValidator : AbstractValidator<UpdateTurmaDto>
{
    public UpdateTurmaDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O Nome é obrigatório.")
            .MinimumLength(3).WithMessage("O Nome deve ter no mínimo 3 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A Descrição é obrigatória.");
    }
}