using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurmasController : ControllerBase
{
    private readonly ITurmaService _turmaService;

    public TurmasController(ITurmaService turmaService)
    {
        _turmaService = turmaService;
    }

    /// <summary>
    /// Retorna um Turma pelo seu identificador único.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetById(int id)
    {
        TurmaDto turma = await _turmaService.GetByIdAsync(id);

        if (turma == null)
        {
            return NotFound();
        }

        return Ok(turma);
    }

    /// <summary>
    /// Retorna todas os Turmas cadastrados no sistema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        PagedResult<TurmaDto> turmas = await _turmaService.GetAllAsync(pageNumber, pageSize);

        return Ok(turmas);
    }

    /// <summary>
    /// Cria um novo registro de um Turma.
    /// Só pode ser executado por um Administrador. 
    /// </summary>
    /// <param name="createTurmaDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Post([FromBody] TurmaDto createTurmaDto)
    {
        await _turmaService.AddAsync(createTurmaDto);

        return CreatedAtAction(nameof(GetAll), null);
    }

    /// <summary>
    /// Atualiza um registro de um Turma.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="TurmaDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Put([FromBody] TurmaDto turmaDto)
    {
        await _turmaService.UpdateAsync(turmaDto);

        return NoContent();
    }

    /// <summary>
    /// Deleta o registro do Turma usando um id específico.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _turmaService.DeleteAsync(id);
        return NoContent();
    }
}