using Core.Application.DTOs;
using Core.Application.Validators.Common;
using FluentValidation;

namespace Core.Application.Validators;
public class CreateAlunoDtoValidator : AbstractValidator<CreateAlunoDto>
{
    public CreateAlunoDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório e não pode ser nulo.")
            .Length(3, 100).WithMessage("O nome deve ter entre 3 e 100 caracteres.")
            .Must(ValidatorsTool.IsValidName)
            .WithMessage("O nome não pode conter números ou caracteres especiais.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("O CPF é obrigatório e não pode ser nulo.")
            .Must(ValidatorsTool.IsValidCpf)
            .WithMessage("O CPF informado é inválido.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório e não pode ser nulo.")
            .Must(ValidatorsTool.IsValidEmail)
            .WithMessage("O email deve ser válido.");

        RuleFor(x => x.DataNascimento)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória e não pode ser nula.")
            .Must(ValidatorsTool.IsValidBirthDate)
            .WithMessage("A data de nascimento deve ser no passado.");
    }
}
