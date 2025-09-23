using API; // Para o MigrationExtensions
using Core.Application.Interfaces;
using Core.Application.Services;
using Core.Application.Validators;
using Core.Domain.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DE SERVIÇOS ---

// Adiciona os serviços essenciais do ASP.NET Core
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuração do EF Core e do Identity (com política de senha RF07)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        // Habilita a resiliência de conexão (bom para o Docker)
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configuração de Senha conforme o requisito RF07
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configuração de Autenticação JWT (RF10)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Configuração de Autorização com a Política de Admin (RF10)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// --- 2. INJEÇÃO DE DEPENDÊNCIA (IoC) ---

// Registra os serviços e repositórios da aplicação (Boas práticas DDD)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAlunoRepository, AlunoRepository>();
builder.Services.AddScoped<IAlunoService, AlunoService>();
builder.Services.AddScoped<ITurmaRepository, TurmaRepository>();
builder.Services.AddScoped<ITurmaService, TurmaService>();
builder.Services.AddScoped<IMatriculaRepository, MatriculaRepository>();
builder.Services.AddScoped<IMatriculaService, MatriculaService>();

// --- 3. CONFIGURAÇÃO DO FLUENTVALIDATION ---

// REMOVEMOS todos os AddScoped<IValidator> manuais.
// As duas linhas abaixo fazem todo o trabalho automaticamente.

// Habilita a validação automática nos Controllers para DTOs (cumpre RF03, RF04)
builder.Services.AddFluentValidationAutoValidation();

// Registra TODOS os validadores que estão no projeto Core.Application
builder.Services.AddValidatorsFromAssemblyContaining<CreateAlunoDtoValidator>();

// --- 4. CONFIGURAÇÃO DO SWAGGER ---

// A configuração do Swagger só precisa ser feita uma vez
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FiapAPIRestful", Version = "v1" });
    
    // Configuração para o botão "Authorize" do Swagger usar JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { /* ... */ });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement() { /* ... */ });

    // Configuração para o Swagger ler os comentários XML (RNF03)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


// --- CONSTRUÇÃO E PIPELINE DA APLICAÇÃO ---

var app = builder.Build();

// Bloco de inicialização para aplicar migrações e seed (importante para Docker)
try
{
    app.ApplyMigrations();
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await SeedData.Initialize(services); // Cria os registros iniciais (RF10)
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Ocorreu um erro durante a inicialização (migração/seed) do banco de dados.");
}

// Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();