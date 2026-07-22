namespace CarCredit.Application.DTOs.Queries;

public record OverdueInstallment(
    string LoanReference,
    int Number,
    decimal Amount,
    DateTime DateExpiration,
    string Customer,
    string Vehicle,
    int DaysOverdue
);