using CarCredit.Application.DTOs.Responses;

namespace CarCredit.Application.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentResponse>> GetByInstallmentReference(string paymentReference);
    Task<IEnumerable<PaymentResponse>> GetByLoanReference(string loanReference);
}