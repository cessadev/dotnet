using CarCredit.Domain.Enums;

namespace CarCredit.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public EVehicleBrand Brand { get; set; }
    public string Model { get; set; } = string.Empty;
    public decimal MarketValue { get; set; }
    public int Year { get; set; }
}