using CarCredit.Domain.Entities;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.Services;

public class InstallmentService : IInstallmentService
{
    private readonly IInstallmentRepository _installmentRepository;
    
    public InstallmentService(IInstallmentRepository installmentRepository)
    {
        _installmentRepository = installmentRepository;
    }

    public async Task<IEnumerable<InstallmentResponse>> GetAllByLoanReference(string loanReference)
    {
        IEnumerable<Installment> installments = await _installmentRepository.GetByLoanReference(loanReference);
        return installments.Select(ToResponse);
    }

    public async Task<InstallmentResponse?> RegisterPayment(string paymentReference, EPaymentMethod method, decimal amount)
    {
        Installment? installment = await _installmentRepository.GetByPaymentReference(paymentReference);
        if (installment is null) return null;

        if (installment.Paid)
            throw new InvalidOperationException(
                $"La cuota {installment.Number} con referencia de pago {installment.PaymentReference} ya ha sido pagada.");

        decimal roundedAmount = Math.Round(amount, 2, MidpointRounding.AwayFromZero);
        decimal remainingBalance = Math.Round(installment.Amount - installment.AmountPaid, 2, MidpointRounding.AwayFromZero);

        if (roundedAmount > remainingBalance)
            throw new InvalidOperationException(
                $"El monto a abonar ({roundedAmount:C}) supera el saldo pendiente de la cuota ({remainingBalance:C}).");

        Payment payment = new Payment
        {
            Number = $"PAY-{Guid.NewGuid().ToString("N")[..8].ToUpper()}",
            Amount = roundedAmount,
            Method = method,
            ReferencePayment = paymentReference,
            Date = DateTime.UtcNow,
            InstallmentId = installment.Id
        };

        installment.AmountPaid += roundedAmount;
        installment.DatePayment = DateTime.UtcNow;
        installment.Paid = installment.AmountPaid >= installment.Amount;

        await _installmentRepository.AddPayment(payment);
        await _installmentRepository.SaveChanges();

        return ToResponse(installment);
    }

    public async Task<LoanSummary?> GetSummaryByLoanReference(string loanReference)
        => await _installmentRepository.GetSummaryByLoanReference(loanReference);

    public async Task<IEnumerable<OverdueInstallment>> GetOverdueByLoanReference(string loanReference)
        => await _installmentRepository.GetOverdueByLoanReference(loanReference);

    public async Task<IEnumerable<OverdueInstallment>> GetAllOverdue()
        => await _installmentRepository.GetAllOverdue();

    private static InstallmentResponse ToResponse(Installment i) => new(
        i.Loan.Reference,
        i.Number,
        i.PaymentReference,
        i.Amount,
        i.AmountPaid,
        i.DateExpiration,
        i.DatePayment,
        i.Paid
    );
}