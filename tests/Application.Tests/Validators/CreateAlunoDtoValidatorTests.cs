using Core.Application.DTOs;
using Core.Application.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace Core.Application.Tests.Validators;

public class CreateAlunoDtoValidatorTests
{
    private readonly CreateAlunoDtoValidator _validator;

    public CreateAlunoDtoValidatorTests()
    {
        _validator = new CreateAlunoDtoValidator();
    }

    [Fact]
    public void Deve_Falhar_Se_Nome_For_Vazio()
    {
        var dto = new CreateAlunoDto { Nome = "", Email = "teste@teste.com", Cpf = "12345678901", DataNascimento = DateTime.Today.AddYears(-20) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Deve_Falhar_Se_Cpf_Tiver_Tamanho_Incorreto()
    {
        var dto = new CreateAlunoDto { Nome = "Andressa", Email = "teste@teste.com", Cpf = "123", DataNascimento = DateTime.Today.AddYears(-20) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Cpf);
    }

    [Fact]
    public void Deve_Passar_Se_Todos_Campos_Forem_Validos()
    {
        var dto = new CreateAlunoDto { Nome = "Andressa", Email = "teste@teste.com", Cpf = "12345678901", DataNascimento = DateTime.Today.AddYears(-20) };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
