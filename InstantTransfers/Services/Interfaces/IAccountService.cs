using System;
using InstantTransfers.DTOs.Account;

namespace InstantTransfers.Services.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<AccountResponseDto>> GetAllAsync();
    Task<AccountResponseDto?> GetByIdAsync(long id);
    Task<AccountResponseDto> CreateAsync(AccountCreateDto dto);
    Task<AccountResponseDto?> UpdateAsync(long id, AccountUpdateDto dto);
    Task<bool> DeleteAsync(long id);

}
