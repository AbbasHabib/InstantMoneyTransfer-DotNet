using InstantTransfers.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InstantTransfers.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User, IdentityRole<long>, long>(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Composite unique index on FromAccountId + ToAccountId + Timestamp for idempotency
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => new { t.FromAccountId, t.ToAccountId, t.Timestamp })
            .IsUnique();
    }
}