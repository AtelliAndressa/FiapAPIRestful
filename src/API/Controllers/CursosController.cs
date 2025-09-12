using Core.Application.DTOs;
using Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CursosController : ControllerBase
{
    private readonly ICursoService _cursoService;

    public CursosController(ICursoService cursoService)
    {
        _cursoService = cursoService;
    }

    /// <summary>
    /// Retorna um curso pelo seu identificador único.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetById(int id)
    {
        var curso = await _cursoService.GetByIdAsync(id);

        if (curso == null)
        {
            return NotFound();
        }

        return Ok(curso);
    }

    /// <summary>
    /// Retorna todas os cursos cadastrados no sistema.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAll()
    {
        var cursos = await _cursoService.GetAllAsync();

        return Ok(cursos);
    }

    /// <summary>
    /// Cria um novo registro de um curso.
    /// Só pode ser executado por um Administrador. 
    /// </summary>
    /// <param name="createCursoDto"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Post([FromBody] CursoDto createCursoDto)
    {
        await _cursoService.AddAsync(createCursoDto);

        return CreatedAtAction(nameof(GetAll), null);
    }

    /// <summary>
    /// Atualiza um registro de um curso.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="cursoDto"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Put([FromBody] CursoDto cursoDto)
    {
        await _cursoService.UpdateAsync(cursoDto);

        return NoContent();
    }

    /// <summary>
    /// Deleta o registro do curso usando um id específico.
    /// Só pode ser executado por um Administrador.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _cursoService.DeleteAsync(id);
        return NoContent();
    }
}