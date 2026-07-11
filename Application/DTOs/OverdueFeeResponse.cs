namespace CarCredit.Application.DTOs;

public record OverdueFeeResponse(
    int Id,
    int NumberFee,
    decimal ValueFee,
    DateTime DateExpiration,
    string Customer,
    string Vehicle,
    int DaysOverdue
);