using CarCredit.Domain.Entities;
using CarCredit.Application.DTOs.Queries;

namespace CarCredit.Application.Interfaces;

public interface IInstallmentRepository
{
    Task<Installment?> GetByPaymentReference(string paymentReference);
    Task<IEnumerable<Installment>> GetByLoanReference(string loanReference);
    Task<LoanSummary?> GetSummaryByLoanReference(string loanReference);
    Task<IEnumerable<OverdueInstallment>> GetOverdueByLoanReference(string loanReference);
    Task<IEnumerable<OverdueInstallment>> GetAllOverdue();
    Task AddPayment(Payment payment);
    Task SaveChanges();
}