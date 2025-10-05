namespace InstantTransfers.DTOs.Account;

public record AccountResponseDto(
    long Id,
    long UserId,
    decimal Balance,
    DateTime CreatedAt
);
