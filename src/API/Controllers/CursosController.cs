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

    [HttpGet]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAll()
    {
        var cursos = await _cursoService.GetAllAsync();

        return Ok(cursos);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Post([FromBody] CursoDto createCursoDto)
    {
        await _cursoService.AddAsync(createCursoDto);

        return CreatedAtAction(nameof(GetAll), null);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Put([FromBody] CursoDto cursoDto)
    {
        await _cursoService.UpdateAsync(cursoDto);

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _cursoService.DeleteAsync(id);
        return NoContent();
    }
}