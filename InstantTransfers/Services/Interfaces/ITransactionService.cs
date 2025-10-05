using System;
using InstantTransfers.DTOs.Transaction;

namespace InstantTransfers.Services.Interfaces;

public interface ITransactionService
{
    Task<IEnumerable<TransactionResponseDto>> GetAllAsync();
    Task<TransactionResponseDto?> GetByIdAsync(long id);
    Task<TransactionResponseDto> CreateAsync(TransactionCreateDto dto);
    Task<bool> DeleteAsync(long id);
}
