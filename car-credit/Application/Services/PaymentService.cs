using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.Interfaces;
using CarCredit.Domain.Entities;

namespace CarCredit.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<IEnumerable<PaymentResponse>> GetByInstallmentReference(string paymentReference)
    {
        IEnumerable<Payment> payments = await _paymentRepository.GetByInstallmentReference(paymentReference);
        return payments.Select(ToResponse);
    }

    public async Task<IEnumerable<PaymentResponse>> GetByLoanReference(string loanReference)
    {
        IEnumerable<Payment> payments = await _paymentRepository.GetByLoanReference(loanReference);
        return payments.Select(ToResponse);
    }

    private static PaymentResponse ToResponse(Payment p) => new(
        p.Number,
        p.Amount,
        p.Method,
        p.ReferencePayment,
        p.Date,
        p.Installment.Number,
        p.Installment.Loan.Reference
    );
}