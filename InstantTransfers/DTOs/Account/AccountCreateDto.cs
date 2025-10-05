using System.ComponentModel.DataAnnotations;

namespace InstantTransfers.DTOs.Account;

public record AccountCreateDto(
    [Required]
    long UserId,
    [Range(0, double.MaxValue)] decimal InitialBalance
);
