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
    /// Recupera um aluno pelo seu identificador único.
    /// </summary>
    /// <remarks>Este método executa uma operação assíncrona para buscar os dados do aluno. 
    /// Certifique-se de que o <paramref name="id"/> fornecido corresponda a um registro de aluno válido.</remarks>
    /// <param name="id">O identificador único do aluno a ser recuperado.</param>
    /// <returns>Um <see cref="IActionResult"/> contendo os dados do aluno, se encontrado; 
    /// caso contrário, um <see cref="NotFoundResult"/> se nenhum aluno com o identificador especificado existir.</returns>
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
    /// Recupera todos os alunos cadastrados.
    /// </summary>
    /// <remarks>Este método executa uma operação assíncrona para buscar a lista completa de alunos.</remarks>
    /// <returns>Um <see cref="IActionResult"/> contendo a lista de alunos.</returns>
    [HttpGet]
    [Authorize(Policy = "AdminOrUser")]
    public async Task<IActionResult> GetAll()
    {
        var alunos = await _alunoService.GetAllAsync();

        return Ok(alunos);
    }

    /// <summary>
    /// Cria um novo aluno.
    /// </summary>
    /// <remarks>Este método executa uma operação assíncrona para adicionar um novo aluno 
    /// com base nos dados fornecidos.</remarks>
    /// <param name="createAlunoDto">Objeto contendo os dados necessários para criar o aluno.</param>
    /// <returns>Um <see cref="CreatedAtActionResult"/> indicando que o aluno foi criado com sucesso.</returns>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Post([FromBody] AlunoDto createAlunoDto)
    {
        await _alunoService.AddAsync(createAlunoDto);

        return CreatedAtAction(nameof(GetAll), null);
    }

    /// <summary>
    /// Atualiza os dados de um aluno existente.
    /// </summary>
    /// <remarks>Este método executa uma operação assíncrona para atualizar os dados de um aluno.
    /// <param name="alunoDto">Objeto contendo os dados atualizados do aluno.</param>
    /// <returns>Um <see cref="NoContentResult"/> se a atualização for bem-sucedida;
    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Put([FromBody] AlunoDto alunoDto)
    {
        await _alunoService.UpdateAsync(alunoDto);

        return NoContent();
    }

    /// <summary>
    /// Exclui um aluno existente.
    /// </summary>
    /// <remarks>Este método executa uma operação assíncrona para excluir um aluno 
    /// com base no identificador fornecido.</remarks>
    /// <param name="id">O identificador único do aluno a ser excluído.</param>
    /// <returns>Um <see cref="NoContentResult"/> se o aluno for excluído com sucesso; 
    /// caso contrário, um <see cref="NotFoundResult"/> se o aluno não existir.</returns>
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