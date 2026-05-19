using System.ComponentModel.DataAnnotations.Schema;

namespace CarCredit.Models;

public class Fee
{
    public int Id { get; set; }
    public int CreditId { get; set; }
    public int NumberFee { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValueFee { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ValueFeePaid { get; set; }

    public DateTime DateExpiration { get; set; }
    public DateTime? DatePayment { get; set; }
    public bool Paid { get; set; } = false;

    public Credit Credit { get; set; } = null!;
}