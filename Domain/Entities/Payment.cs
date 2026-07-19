using CarCredit.Domain.Enums;

namespace CarCredit.Domain.Entities;

public class Payment
{
    public int Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public EPaymentMethod Method { get; set; }
    public string ReferencePayment { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    // FK - Installment
    public int InstallmentId { get; set; }
    public Installment Installment { get; set; } = null!;
}