using System.ComponentModel.DataAnnotations;

namespace InstantTransfers.DTOs.Account;

public record AccountUpdateDto(
    [Range(0, double.MaxValue)] decimal Balance
);
