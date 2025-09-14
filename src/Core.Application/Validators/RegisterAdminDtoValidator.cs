using Core.Application.DTOs;
using Core.Application.Validators.Common;
using FluentValidation;

namespace Core.Application.Validators;

public class RegisterAdminDtoValidator : AbstractValidator<RegisterAdminDto>
{
    public RegisterAdminDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório e não pode ser nulo.")
            .Must(ValidatorsTool.IsValidEmail)
            .WithMessage("O email deve ser válido.");


        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória e não pode ser nula.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .Matches("[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches("[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches("[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches("[^a-zA-Z0-9]").WithMessage("A senha deve conter pelo menos um caractere especial.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("As senhas não conferem.");
    }
}