using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.Interfaces;

public interface IInstallmentService
{
    Task<IEnumerable<InstallmentResponse>> GetAllByLoanReference(string loanReference);
    Task<InstallmentResponse?> RegisterPayment(string paymentReference, EPaymentMethod method, decimal amount);
    Task<LoanSummary?> GetSummaryByLoanReference(string loanReference);
    Task<IEnumerable<OverdueInstallment>> GetOverdueByLoanReference(string loanReference);
    Task<IEnumerable<OverdueInstallment>> GetAllOverdue();
}