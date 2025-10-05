using InstantTransfers.DTOs.Account;
using InstantTransfers.Services;
using InstantTransfers.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InstantTransfers.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAccountService _service) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountResponseDto>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AccountResponseDto>> GetById(long id)
    {
        var acc = await _service.GetByIdAsync(id);
        return acc is null ? NotFound() : Ok(acc);
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponseDto>> Create(AccountCreateDto dto)
    {
        var acc = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = acc.Id }, acc);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AccountResponseDto>> Update(long id, AccountUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? Ok($"deleted account id {id} successfully") : NotFound();
    }
}
