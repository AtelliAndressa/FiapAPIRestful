using Core.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Validators;

public class CreateCursoDtoValidation : AbstractValidator<CreateCursoDto>
{
    public CreateCursoDtoValidation()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do curso é obrigatório.")
            .Length(3, 100).WithMessage("O nome do curso deve ter entre 3 e 100 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição do curso é obrigatória.")
            .Length(10, 500).WithMessage("A descrição do curso deve ter entre 10 e 500 caracteres.");
    }
}

