using InstantTransfers.DTOs.Transaction;
using InstantTransfers.Services;
using Microsoft.AspNetCore.Mvc;

namespace InstantTransfers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly TransactionService _service;

    public TransactionController(TransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionResponseDto>> GetById(long id)
    {
        var tx = await _service.GetByIdAsync(id);
        return tx is null ? NotFound() : Ok(tx);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionResponseDto>> Create(TransactionCreateDto dto)
    {
        try
        {
            var tx = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = tx.Id }, tx);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
