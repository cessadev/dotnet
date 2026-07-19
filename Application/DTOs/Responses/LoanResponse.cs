namespace CarCredit.Application.DTOs.Responses;

public record LoanResponse(
    int Id,
    int CustomerId,
    string Vehicle,
    decimal ValueCredit,
    int Fee
);