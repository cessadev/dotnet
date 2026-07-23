namespace CarCredit.Domain.Entities;

public class Installment
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string PaymentReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime DateExpiration { get; set; }
    public DateTime? DatePayment { get; set; }
    public bool Paid { get; set; } = false;

    // FK - Loan
    public int LoanId { get; set; }
    public Loan Loan { get; set; } = null!;
}