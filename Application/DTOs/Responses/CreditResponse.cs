namespace CarCredit.Application.DTOs.Responses;

public record CreditResponse (
    int Id,
    int CustomerId,
    string Vehicle,
    decimal ValueCredit,
    int Fee
);