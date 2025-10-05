namespace InstantTransfers.DTOs.Transaction;

public record TransactionResponseDto(
    long Id,
    long FromAccountId,
    long ToAccountId,
    decimal Amount,
    DateTime Timestamp
);
