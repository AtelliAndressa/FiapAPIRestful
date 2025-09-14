using Core.Application.DTOs;
using Core.Application.Interfaces;
using Core.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatriculasController : ControllerBase
{
    private readonly IMatriculaService _matriculaService;

    public MatriculasController(IMatriculaService matriculaService)
    {
        _matriculaService = matriculaService;
    }

    /// <summary>
    /// Retorna todas as matrículas cadastradas no sistema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        PagedResult<MatriculaDto> matriculas = await _matriculaService.GetAllAsync(pageNumber, pageSize);

        return Ok(matriculas);
    }

    /// <summary>
    /// Retorna todas as matriculas vinculadas a um aluno.
    /// </summary>
    /// <param name="alunoId"></param>
    /// <returns></returns>
    [HttpGet("aluno/{alunoId:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetByAlunoId(int alunoId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        PagedResult<MatriculaDto> matriculas = await _matriculaService.GetByStudentIdAsync(alunoId, pageNumber, pageSize);

        if (matriculas == null || !matriculas.Items.Any())
        {
            return NotFound("Nenhuma matrícula encontrada para este aluno.");
        }

        return Ok(matriculas);
    }

    /// <summary>
    /// Retorna todas as matriculas vinculadas a um Turma.
    /// </summary>
    /// <param name="TurmaId"></param>
    /// <returns></returns>
    [HttpGet("Turma/{TurmaId:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetByTeamId(int turmaId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        PagedResult<MatriculaDto> matriculas = await _matriculaService.GetByTeamIdAsync(turmaId, pageNumber, pageSize);

        if (matriculas == null || !matriculas.Items.Any())
        {
            return NotFound("Nenhuma matrícula encontrada para esta turma.");
        }

        return Ok(matriculas);
    }

    /// <summary>
    /// Cria um novo registro de matrícula.
    /// Só pode ser executado por um Administrador.    
    /// </summary>
    /// <param name="matriculaDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Post([FromBody] CreateMatriculaDto matriculaDto)
    {

        await _matriculaService.AddAsync(matriculaDto);

        return CreatedAtAction(nameof(GetAll), null);
    }

    /// <summary>
    /// Atualiza um registro de uma matricula.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="matriculaDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Put([FromBody] MatriculaDto matriculaDto)
    {
        await _matriculaService.UpdateAsync(matriculaDto);

        return NoContent();
    }

    /// <summary>
    /// Deleta o registro da matrícula usando um id específico.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _matriculaService.DeleteAsync(id);

        return NoContent();
    }
}