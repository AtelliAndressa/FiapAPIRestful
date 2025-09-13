using Xunit;
using FluentAssertions;
using Core.Application.Validators;
using Core.Application.DTOs;
using FluentValidation.TestHelper;

namespace Application.Tests.Validators
{
    public class CreateTurmaDtoValidatorTests
    {
        private readonly CreateTurmaDtoValidation _validator;

        public CreateTurmaDtoValidatorTests()
        {
            _validator = new CreateTurmaDtoValidation();
        }

        [Fact]
        public void Should_Have_Error_When_Nome_Is_Less_Than_3_Chars()
        {
            var dto = new CreateTurmaDto("ab", "Uma descrição válida");

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(turma => turma.Nome);
        }

        [Fact]
        public void Should_Have_Error_When_Descricao_Is_Empty()
        {
            var dto = new CreateTurmaDto("Nome Válido", "");

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(turma => turma.Descricao);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Dto_Is_Valid()
        {
            var dto = new CreateTurmaDto("Nome Super Válido", "Descrição Válida");

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}