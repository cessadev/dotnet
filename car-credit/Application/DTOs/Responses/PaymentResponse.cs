using CarCredit.Domain.Enums;

namespace CarCredit.Application.DTOs.Responses;

public record PaymentResponse(
    string Number,
    decimal Amount,
    EPaymentMethod Method,
    string ReferencePayment,
    DateTime Date,
    int InstallmentNumber,
    string LoanReference
);