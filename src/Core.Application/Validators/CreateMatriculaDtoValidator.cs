using Core.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Validators;

public class CreateMatriculaDtoValidator : AbstractValidator<CreateMatriculaDto>
{
    public CreateMatriculaDtoValidator()
    {
        RuleFor(x => x.AlunoId)
            .GreaterThan(0).WithMessage("O AlunoId deve ser maior que zero.");

        RuleFor(x => x.CursoId)
            .GreaterThan(0).WithMessage("O CursoId deve ser maior que zero.");

        RuleFor(x => x.DataMatricula)
            .NotEmpty().WithMessage("A data de matrícula é obrigatória.")
            .Must(data => data <= DateTime.Now)
            .WithMessage("A data de matrícula não pode ser no futuro.");
    }
}
