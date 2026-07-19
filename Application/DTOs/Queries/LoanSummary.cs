namespace CarCredit.Application.DTOs.Queries;

public record LoanSummary(
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