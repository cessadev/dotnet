namespace CarCredit.Application.DTOs.Queries;

public record LoanSummary(
    string Reference,
    string Customer,
    string Vehicle,
    int TotalInstallments,
    int InstallmentsPaid,
    int InstallmentsOwed,
    decimal TotalValue,
    decimal TotalPaid,
    decimal TotalOwed
);