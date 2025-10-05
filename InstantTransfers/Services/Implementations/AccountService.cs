using InstantTransfers.DTOs.Account;
using InstantTransfers.Infrastructure;
using InstantTransfers.Models;
using InstantTransfers.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InstantTransfers.Services.Implementations;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AccountResponseDto>> GetAllAsync()
    {
        var accounts = await _context.Accounts.ToListAsync();
        return accounts.Select(a => new AccountResponseDto(a.Id, a.UserId, a.Balance, a.CreatedAt));
    }

    public async Task<AccountResponseDto?> GetByIdAsync(long id)
    {
        var acc = await _context.Accounts.FindAsync(id);
        return acc is null ? null : new AccountResponseDto(acc.Id, acc.UserId, acc.Balance, acc.CreatedAt);
    }

    public async Task<AccountResponseDto> CreateAsync(AccountCreateDto dto)
    {
        var account = new Account
        {
            UserId = dto.UserId,
            Balance = dto.InitialBalance
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountResponseDto(account.Id, account.UserId, account.Balance, account.CreatedAt);
    }

    public async Task<AccountResponseDto?> UpdateAsync(long id, AccountUpdateDto dto)
    {
        var acc = await _context.Accounts.FindAsync(id);
        if (acc is null) return null;

        acc.Balance = dto.Balance;
        await _context.SaveChangesAsync();

        return new AccountResponseDto(acc.Id, acc.UserId, acc.Balance, acc.CreatedAt);
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var acc = await _context.Accounts.FindAsync(id);
        if (acc is null) return false;

        _context.Accounts.Remove(acc);
        await _context.SaveChangesAsync();
        return true;
    }
}
