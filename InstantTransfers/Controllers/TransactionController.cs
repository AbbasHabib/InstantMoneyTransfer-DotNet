using InstantTransfers.CustomExceptions;
using InstantTransfers.DTOs.Transaction;
using InstantTransfers.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstantTransfers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionController(ITransactionService _service) : ControllerBase
{

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
        catch(TransactionFailedException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception e)
        {
            // TODO: only for debugging, remove in production
            return BadRequest("An error occurred while processing the transaction. " + e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
