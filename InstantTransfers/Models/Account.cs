using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTransfers.Models;

public class Account
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public long UserId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Range(0, double.MaxValue)]
    public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

