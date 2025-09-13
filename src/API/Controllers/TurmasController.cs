using Core.Application.DTOs;
using Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TurmasController : ControllerBase
{
    private readonly ITurmaService _TurmaService;

    public TurmasController(ITurmaService TurmaService)
    {
        _TurmaService = TurmaService;
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
        var Turma = await _TurmaService.GetByIdAsync(id);

        if (Turma == null)
        {
            return NotFound();
        }

        return Ok(Turma);
    }

    /// <summary>
    /// Retorna todas os Turmas cadastrados no sistema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAll()
    {
        var Turmas = await _TurmaService.GetAllAsync();

        return Ok(Turmas);
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
        await _TurmaService.AddAsync(createTurmaDto);

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
    public async Task<IActionResult> Put([FromBody] TurmaDto TurmaDto)
    {
        await _TurmaService.UpdateAsync(TurmaDto);

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
        await _TurmaService.DeleteAsync(id);
        return NoContent();
    }
}