using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstantTransfers.Models;

public class Transaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public long FromAccountId { get; set; }

    [Required]
    public long ToAccountId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string RequestId { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey(nameof(FromAccountId))]
    public virtual Account FromAccount { get; set; } = null!;

    [ForeignKey(nameof(ToAccountId))]
    public virtual Account ToAccount { get; set; } = null!;
}
