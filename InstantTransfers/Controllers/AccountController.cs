using System.Threading.Tasks;
using InstantTransfers.Infrastructure;
using InstantTransfers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstantTransfers.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    // TODO: Abstract the DbContext usage into a service
    public class AccountController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;


        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAccounts()
        {
            return Ok( await _context.Accounts.ToListAsync());
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(long id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount(Account acc)
        {
            if (acc == null || acc.UserId < 0)
            {
                return BadRequest("Invalid account data.");
            }

            _context.Accounts.Add(acc);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAccount), new { id = acc.Id }, acc);
        }
    }
}
