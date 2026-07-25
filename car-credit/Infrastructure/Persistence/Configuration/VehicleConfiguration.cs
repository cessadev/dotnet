using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Configuration;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.Property(v => v.MarketValue).HasColumnType("decimal(18,2)");
    }
}