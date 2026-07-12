using Microsoft.EntityFrameworkCore;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Credit> Credits => Set<Credit>();
    public DbSet<Fee> Fees => Set<Fee>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Fee>()
            .HasOne(c => c.Credit)
            .WithMany()
            .HasForeignKey(i => i.CreditId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Credit>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}