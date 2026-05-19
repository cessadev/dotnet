using System.ComponentModel.DataAnnotations.Schema;

namespace CarCredit.Models;

public class Credit
{
    public int Id { get; set; }
    public string Vehicle { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValueCredit { get; set; }
    public int Fee { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    // FK - Customer
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}

public record CreateCreditRequest(
    int CustomerId,
    string Vehicle,
    decimal ValueCredit,
    int Fee
);