using InstantTransfers.DTOs.Transaction;
using InstantTransfers.Infrastructure;
using InstantTransfers.Models;
using InstantTransfers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InstantTransfers.Services.Implementations;

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _context;

    public TransactionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TransactionResponseDto>> GetAllAsync()
    {
        var txs = await _context.Transactions.ToListAsync();
        return txs.Select(t => new TransactionResponseDto(
            t.Id, t.FromAccountId, t.ToAccountId, t.Amount, t.Timestamp));
    }

    public async Task<TransactionResponseDto?> GetByIdAsync(long id)
    {
        var t = await _context.Transactions.FindAsync(id);
        return t is null
            ? null
            : new TransactionResponseDto(t.Id, t.FromAccountId, t.ToAccountId, t.Amount, t.Timestamp);
    }

 public async Task<TransactionResponseDto> CreateAsync(TransactionCreateDto dto)
    {
        if (dto.FromAccountId == dto.ToAccountId)
            throw new InvalidOperationException("Cannot transfer to the same account.");

        await using var dbTransaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // // Check idempotency before locking

            var lockIds = new[] { dto.FromAccountId, dto.ToAccountId }.OrderBy(id => id).ToArray();

            await _context.Database.ExecuteSqlRawAsync(@"
                SELECT ""Id"" FROM ""Accounts""
                WHERE ""Id"" IN ({0}, {1})
                FOR UPDATE;
            ", lockIds[0], lockIds[1]);

            int debitResult = await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE ""Accounts""
                SET ""Balance"" = ""Balance"" - {0}
                WHERE ""Id"" = {1} AND ""Balance"" >= {0};
            ", dto.Amount, dto.FromAccountId);

            if (debitResult == 0)
                throw new InvalidOperationException("Insufficient funds or concurrent modification.");

            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE ""Accounts""
                SET ""Balance"" = ""Balance"" + {0}
                WHERE ""Id"" = {1};
            ", dto.Amount, dto.ToAccountId);

            // Record the transaction
            var tx = new Transaction
            {
                FromAccountId = dto.FromAccountId,
                ToAccountId = dto.ToAccountId,
                Amount = dto.Amount,
                Timestamp = dto.Timestamp
            };

            _context.Transactions.Add(tx);
            await _context.SaveChangesAsync();

            await dbTransaction.CommitAsync();

            return new TransactionResponseDto(
                tx.Id, tx.FromAccountId, tx.ToAccountId, tx.Amount, tx.Timestamp
            );
        }
        catch
        {
            await dbTransaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var tx = await _context.Transactions.FindAsync(id);
        if (tx == null) return false;

        _context.Transactions.Remove(tx);
        await _context.SaveChangesAsync();
        return true;
    }
}
