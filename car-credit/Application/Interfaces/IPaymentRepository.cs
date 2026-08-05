using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetByInstallmentReference(string paymentReference);
    Task<IEnumerable<Payment>> GetByLoanReference(string loanReference);
}