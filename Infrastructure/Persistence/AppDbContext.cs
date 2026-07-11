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
            .HasForeignKey(c => c.CreditId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Credit>()
            .HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}