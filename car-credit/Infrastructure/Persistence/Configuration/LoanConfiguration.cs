using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Configuration;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.Property(l => l.Amount).HasColumnType("decimal(18,2)");
        builder.Property(l => l.InterestRate).HasColumnType("decimal(5,4)");
        builder.Property(l => l.TotalAmount).HasColumnType("decimal(18,2)");
    }
}