using Core.Application.DTOs;
using FluentValidation;

namespace Core.Application.Validators;

public class CreateTurmaDtoValidation : AbstractValidator<CreateTurmaDto>
{
    public CreateTurmaDtoValidation()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do Turma é obrigatório.")
            .Length(3, 100).WithMessage("O nome do Turma deve ter entre 3 e 100 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do Turma é obrigatória.")
            .Length(10, 500).WithMessage("A descrição do Turma deve ter entre 10 e 500 caracteres.");
    }
}

