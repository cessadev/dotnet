namespace CarCredit.Application.DTOs.Responses;

public record InstallmentResponse(
    string LoanReference,
    int Number,
    string PaymentReference,
    decimal Amount,
    decimal AmountPaid,
    DateTime DateExpiration,
    DateTime? DatePayment,
    bool Paid
);