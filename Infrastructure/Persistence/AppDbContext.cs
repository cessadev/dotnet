using Microsoft.EntityFrameworkCore;
using CarCredit.Domain.Entities;
using CarCredit.Infrastructure.Persistence.Configuration;

namespace CarCredit.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Installment> Installments => Set<Installment>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Installment>()
            .HasOne(i => i.Loan)
            .WithMany()
            .HasForeignKey(i => i.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Customer)
            .WithMany()
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Brand)
            .HasConversion<string>();
        
        modelBuilder.Entity<Payment>()
            .Property(p => p.Method)
            .HasConversion<string>();


        modelBuilder.Entity<Installment>()
            .HasIndex(i => i.PaymentReference)
            .IsUnique();


        modelBuilder.ApplyConfiguration(new LoanConfiguration());
        modelBuilder.ApplyConfiguration(new InstallmentConfiguration());
    }
}