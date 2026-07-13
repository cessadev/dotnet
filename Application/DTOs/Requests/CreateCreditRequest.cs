namespace CarCredit.Application.DTOs.Requests;

public record CreateCreditRequest(
    int CustomerId,
    string Vehicle,
    decimal ValueCredit,
    int Fee
);