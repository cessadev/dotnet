namespace CarCredit.Application.DTOs.Queries;

public record OverdueInstallment(
    int Id,
    int NumberFee,
    decimal ValueFee,
    DateTime DateExpiration,
    string Customer,
    string Vehicle,
    int DaysOverdue
);