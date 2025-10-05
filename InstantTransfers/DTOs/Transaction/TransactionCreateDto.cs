using System.ComponentModel.DataAnnotations;

namespace InstantTransfers.DTOs.Transaction;

public record TransactionCreateDto(
    [Required] long FromAccountId,
    [Required] long ToAccountId,
    [Range(0.01, double.MaxValue)] decimal Amount,
    [Required] DateTime Timestamp
);
