using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Queries;

namespace CarCredit.Application.Interfaces;

public interface ILoanService
{
    Task<LoanResponse> Create(CreateLoanRequest request);
    Task<LoanSimulation> Simulate(SimulateLoanRequest request);
    Task<IEnumerable<LoanResponse>> GetAll();
    Task<LoanResponse?> GetByReference(string reference);
    Task<bool> Delete(string reference);
}