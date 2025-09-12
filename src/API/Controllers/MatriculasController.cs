using Core.Application.DTOs;
using Core.Application.Interfaces;
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
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAll()
    {
        var matriculas = await _matriculaService.GetAllAsync();

        return Ok(matriculas);
    }

    /// <summary>
    /// Retorna todas as matriculas vinculadas a um aluno.
    /// </summary>
    /// <param name="alunoId"></param>
    /// <returns></returns>
    [HttpGet("aluno/{alunoId:int}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetByAlunoId(int alunoId)
    {
        var matriculas = await _matriculaService.GetByStudentIdAsync(alunoId);

        if (!matriculas.Any())
        {
            return NotFound("Nenhuma matrícula encontrada para este aluno.");
        }

        return Ok(matriculas);
    }

    /// <summary>
    /// Retorna todas as matriculas vinculadas a um curso.
    /// </summary>
    /// <param name="cursoId"></param>
    /// <returns></returns>
    [HttpGet("curso/{cursoId:int}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetByCursoId(int cursoId)
    {
        var matriculas = await _matriculaService.GetByCourseIdAsync(cursoId);

        if (!matriculas.Any())
        {
            return NotFound("Nenhuma matrícula encontrada para este curso.");
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
    public async Task<IActionResult> Post([FromBody] MatriculaDto matriculaDto)
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