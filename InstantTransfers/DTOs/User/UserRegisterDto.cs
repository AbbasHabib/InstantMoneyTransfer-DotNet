using System.ComponentModel.DataAnnotations;

namespace InstantTransfers.DTOs.User;

public record UserRegisterDto(
    [Required, MaxLength(50)] string Username,
    [Required, MaxLength(100), EmailAddress] string Email,
    [Required, MinLength(8)] string Password,
    [MaxLength(15)] string? PhoneNumber
);