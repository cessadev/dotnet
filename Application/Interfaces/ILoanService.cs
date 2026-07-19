using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;

namespace CarCredit.Application.Interfaces;

public interface ILoanService
{
    Task<LoanResponse> Create(CreateLoanRequest request);
    Task<IEnumerable<LoanResponse>> GetAll();
    Task<LoanResponse?> GetByReference(string reference);
    Task<bool> Delete(int loanId);
}