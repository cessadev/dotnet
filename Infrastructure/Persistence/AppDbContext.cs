using Microsoft.EntityFrameworkCore;
using CarCredit.Domain.Entities;
using CarCredit.Infrastructure.Persistence.Configuration;

namespace CarCredit.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Installment> Installments => Set<Installment>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Customer)
            .WithMany()
            .HasForeignKey(l => l.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Vehicle)
            .WithMany()
            .HasForeignKey(l => l.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Installment>()
            .HasOne(i => i.Loan)
            .WithMany()
            .HasForeignKey(i => i.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Installment)
            .WithMany()
            .HasForeignKey(p => p.InstallmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Brand)
            .HasConversion<string>();

        modelBuilder.Entity<Payment>()
            .Property(p => p.Method)
            .HasConversion<string>();

        modelBuilder.Entity<Customer>()
            .Property(c => c.DocumentType)
            .HasConversion<string>();

        modelBuilder.Entity<Loan>()
            .Property(l => l.Installments)
            .HasConversion<int>();

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.DocumentNumber)
            .IsUnique();

        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.Identifier)
            .IsUnique();

        modelBuilder.Entity<Loan>()
            .HasIndex(l => l.Reference)
            .IsUnique();

        modelBuilder.Entity<Installment>()
            .HasIndex(i => i.PaymentReference)
            .IsUnique();

        modelBuilder.ApplyConfiguration(new LoanConfiguration());
        modelBuilder.ApplyConfiguration(new InstallmentConfiguration());
        modelBuilder.ApplyConfiguration(new VehicleConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());
    }
}