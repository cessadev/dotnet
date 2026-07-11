namespace CarCredit.Domain.Entities;

public class Credit
{
    public int Id { get; set; }
    public string Vehicle { get; set; } = string.Empty;
    public decimal ValueCredit { get; set; }
    public int Fee { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    // FK - Customer
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}