using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InstantTransfers.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace InstantTransfers.Models;
public class User : IdentityUser<long>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public UserRole Role { get; set; } = UserRole.Customer;

}


