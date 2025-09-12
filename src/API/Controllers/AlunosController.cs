using Core.Application.DTOs;
using Core.Application.Interfaces;
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
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetById(int id)
    {
        var aluno = await _alunoService.GetByIdAsync(id);

        if (aluno == null)
        {
            return NotFound();
        }

        return Ok(aluno);
    }

    /// <summary>
    /// Retorna todos os alunos cadastrados no sistema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAll()
    {
        var alunos = await _alunoService.GetAllAsync();

        return Ok(alunos);
    }

    /// <summary>
    /// Cadastra um novo aluno no sistema.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="createAlunoDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Post([FromBody] AlunoDto createAlunoDto)
    {
        await _alunoService.AddAsync(createAlunoDto);

        return CreatedAtAction(nameof(GetAll), null);
    }

    /// <summary>
    /// Atualiza os dados de um aluno existente no sistema.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="alunoDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Put([FromBody] AlunoDto alunoDto)
    {
        await _alunoService.UpdateAsync(alunoDto);

        return NoContent();
    }

    /// <summary>
    /// Exclui um aluno existente no sistema.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _alunoService.DeleteAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}