using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Queries;

namespace CarCredit.Application.Interfaces;

public interface IInstallmentService
{
    Task<IEnumerable<InstallmentResponse>> GetAllByLoanReference(string loanReference);
    Task<InstallmentResponse?> RegisterPayment(string paymentReference, decimal amount);
    Task<LoanSummary?> GetSummaryByLoanReference(string loanReference);
    Task<IEnumerable<OverdueInstallment>> GetOverdueByLoanReference(string loanReference);
    Task<IEnumerable<OverdueInstallment>> GetAllOverdue();
}