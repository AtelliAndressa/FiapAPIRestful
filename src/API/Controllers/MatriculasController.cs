using Core.Application.DTOs;
using Core.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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

    [HttpGet]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAll()
    {
        var matriculas = await _matriculaService.GetAllAsync();

        return Ok(matriculas);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Post([FromBody] MatriculaDto matriculaDto)
    {

        await _matriculaService.AddAsync(matriculaDto);

        return CreatedAtAction(nameof(GetAll), null);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(int id)
    {
        await _matriculaService.DeleteAsync(id);
        return NoContent();
    }
}