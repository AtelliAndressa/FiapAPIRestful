using Core.Application.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;

namespace Application.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var storeMock = new Mock<IUserStore<IdentityUser>>();

            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                storeMock.Object, null, null, null, null, null, null, null, null);

            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c["Jwt:Key"])
                .Returns("MINHA_CHAVE_SECRETA_SUPER_LONGA_E_SEGURA_PARA_HS256");

            _configurationMock.Setup(c => c["Jwt:Issuer"])
                .Returns("https://localhost:7001");

            _configurationMock.Setup(c => c["Jwt:Audience"])
                .Returns("https://localhost:7001");

            _authService = new AuthService(_userManagerMock.Object, _configurationMock.Object);
        }

        /// <summary>
        /// Simula um usuário e senha corretos, garante que o serviço consegue "encontrar" o usuário, 
        /// "validar" sua senha e "ler" as configurações do JWT, retornando com sucesso uma string de token válida.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoginAsync_WhenUserIsValid_ShouldReturnJwtToken()
        {
            var loginDto = new LoginDto("teste@usuario.com", "SenhaForte@123");

            var fakeUser = new IdentityUser { UserName = "teste@usuario.com", Id = "fake-id" };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.Username))
                            .ReturnsAsync(fakeUser);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(fakeUser, loginDto.Password))
                            .ReturnsAsync(true);

            _userManagerMock.Setup(um => um.GetRolesAsync(fakeUser))
                            .ReturnsAsync(new List<string> { "User" });

            var token = await _authService.LoginAsync(loginDto);

            Assert.NotNull(token);
            Assert.IsType<string>(token);
            Assert.Contains("eyJ", token);
        }

        /// <summary>
        /// Simula que o usuário foi encontrado, mas que o CheckPasswordAsync retornou false, e então verifica 
        /// se o serviço corretamente lança a exceção UnauthorizedAccessException.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoginAsync_WhenPasswordIsWrong_ShouldThrowUnauthorizedAccessException()
        {
            var loginDto = new LoginDto("teste@usuario.com", "SenhaErrada!");

            var fakeUser = new IdentityUser { UserName = "teste@usuario.com" };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.Username))
                            .ReturnsAsync(fakeUser);

            _userManagerMock.Setup(um => um.CheckPasswordAsync(fakeUser, loginDto.Password))
                            .ReturnsAsync(false);

            Func<Task> act = async () => await _authService.LoginAsync(loginDto);

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(act);

            Assert.Equal("Usuário ou senha inválidos.", exception.Message);
        }

        /// <summary>
        /// Testa o cenário de falha onde o nome de usuário (e-mail) não existe. Ele simula o 
        /// FindByNameAsync retornando null e verifica se o serviço lança imediatamente a UnauthorizedAccessException.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ShouldThrowUnauthorizedAccessException()
        {            
            var loginDto = new LoginDto("naoexiste@usuario.com", "Senha!");

            _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.Username))
                            .ReturnsAsync((IdentityUser)null);

            Func<Task> act = async () => await _authService.LoginAsync(loginDto);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(act);
        }

        /// <summary>
        /// simula que o FindByNameAsync encontrou um usuário com aquele e-mail e verifica se o serviço lança 
        /// a exceção correta, impedindo o cadastro duplicado.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterUserAsync_WhenUserAlreadyExists_ShouldThrowException()
        {
            var registerDto = new RegisterUserDto("jaexiste@usuario.com", "Senha!", "Senha!");

            var existingUser = new IdentityUser { UserName = "jaexiste@usuario.com" };

            _userManagerMock.Setup(um => um.FindByNameAsync(registerDto.Email))
                            .ReturnsAsync(existingUser);

            Func<Task> act = async () => await _authService.RegisterUserAsync(registerDto);

            var exception = await Assert.ThrowsAsync<Exception>(act);

            Assert.Equal("Já existe um usuário com este email!", exception.Message);
        }

        /// <summary>
        /// Simula um e-mail novo (FindByNameAsync retorna null), um CreateAsync bem-sucedido e um 
        /// AddToRoleAsync bem-sucedido, verificando ao final se ambos os métodos(criação e adição de role) 
        /// foram realmente chamados.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task RegisterUserAsync_WhenUserIsNew_ShouldSucceed()
        {
            var registerDto = new RegisterUserDto("novo@usuario.com", "SenhaForte@123", "SenhaForte@123");

            _userManagerMock.Setup(um => um.FindByNameAsync(registerDto.Email))
                            .ReturnsAsync((IdentityUser)null);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
                            .ReturnsAsync(IdentityResult.Success);

            await _authService.RegisterUserAsync(registerDto);

            _userManagerMock.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password), Times.Once);

            _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"), Times.Once);
        }
    }
}