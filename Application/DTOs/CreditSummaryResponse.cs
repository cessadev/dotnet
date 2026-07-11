namespace CarCredit.Application.DTOs;

public record CreditSummaryResponse(
    int Id,
    string Customer,
    string Vehicle,
    int TotalFees,
    int FeesPaid,
    int FeesOwed,
    decimal TotalValue,
    decimal TotalPaid,
    decimal TotalOwed
);