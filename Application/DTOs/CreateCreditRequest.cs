namespace CarCredit.Application.DTOs;

public record CreateCreditRequest(
    int CustomerId,
    string Vehicle,
    decimal ValueCredit,
    int Fee
);