using System.ComponentModel.DataAnnotations;

namespace InstantTransfers.DTOs.User;

public record UserLoginDto(
    [Required, MaxLength(50)] string Username,
    [Required, MinLength(8)] string Password
);