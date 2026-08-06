using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.Interfaces;

public interface ILoanService
{
    Task<LoanResponse> Create(CreateLoanRequest request);
    Task<LoanSimulation> Simulate(SimulateLoanRequest request);
    Task<IEnumerable<LoanResponse>> GetAll();
    Task<LoanResponse?> GetByReference(string reference);
    Task<IEnumerable<LoanResponse>> GetByCustomer(EDocumentType documentType, int documentNumber);
    Task<bool> Delete(string reference);
}