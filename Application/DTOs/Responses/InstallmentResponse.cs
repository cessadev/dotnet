namespace CarCredit.Application.DTOs.Responses;

public record InstallmentResponse(
    int Id,
    int CreditId,
    int NumberFee,
    decimal ValueFee,
    decimal ValueFeePaid,
    DateTime DateExpiration,
    DateTime? DatePayment,
    bool Paid
);