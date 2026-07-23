using CarCredit.Domain.Enums;

namespace CarCredit.Domain.Entities;

public class Loan
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public EInstallmentsTerm Installments { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    // FK - Customer
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // FK - Vehicle
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;
}