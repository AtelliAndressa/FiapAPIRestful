using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AlunosController : ControllerBase
{
    private readonly IAlunoService _alunoService;

    public AlunosController(IAlunoService alunoService)
    {
        _alunoService = alunoService;
    }

    /// <summary>
    /// Retorna um aluno pelo seu identificador único.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetById(Guid id)
    {
        AlunoDto aluno = await _alunoService.GetByIdAsync(id);

        if (aluno == null)
        {
            return NotFound("Aluno não encontrado.");
        }

        return Ok(aluno);
    }

    /// <summary>
    /// Retorna todos os alunos cadastrados no sistema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        PagedResult<AlunoDto> alunos = await _alunoService.GetAllAsync(pageNumber, pageSize);

        return Ok(alunos);
    }

    /// <summary>
    /// Retorna um aluno cadastrado no sitema pelo seu nome.
    /// </summary>
    /// <param name="nome"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    [HttpGet("search")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> SearchByName(
    [FromQuery] string nome,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            return BadRequest("O parâmetro 'nome' não pode ser vazio.");
        }

        PagedResult<AlunoDto> alunos = await _alunoService.SearchByNameAsync(nome, pageNumber, pageSize);

        return Ok(alunos);
    }

    /// <summary>
    /// Cadastra um novo aluno no sistema.    
    /// </summary>
    /// <param name="createAlunoDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Post([FromBody] CreateAlunoDto createAlunoDto)
    {
        await _alunoService.AddAsync(createAlunoDto);

        return CreatedAtAction(nameof(GetAll), null);
    }

    /// <summary>
    /// Atualiza os dados de um aluno existente no sistema.    
    /// </summary>
    /// <param name="alunoDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Put(Guid id, [FromBody] UpdateAlunoDto alunoDto)
    {
        await _alunoService.UpdateAsync(id, alunoDto);

        return NoContent();
    }

    /// <summary>
    /// Exclui um aluno existente no sistema.    
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id)
    {
        bool result = await _alunoService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}