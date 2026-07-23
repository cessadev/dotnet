using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Configuration;

public class InstallmentConfiguration : IEntityTypeConfiguration<Installment> 
{
    public void Configure(EntityTypeBuilder<Installment> builder)
    {
        builder.Property(i => i.Amount).HasColumnType("decimal(18,2)");
        builder.Property(i => i.AmountPaid).HasColumnType("decimal(18,2)");
    }
}