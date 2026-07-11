namespace CarCredit.Domain.Entities;

public class Fee
{
    public int Id { get; set; }
    public int CreditId { get; set; }
    public int NumberFee { get; set; }
    public decimal ValueFee { get; set; }
    public decimal ValueFeePaid { get; set; }

    public DateTime DateExpiration { get; set; }
    public DateTime? DatePayment { get; set; }
    public bool Paid { get; set; } = false;

    public Credit Credit { get; set; } = null!;
}