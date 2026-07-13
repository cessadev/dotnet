using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;

namespace CarCredit.Application.Interfaces;

public interface ICreditService
{
    Task<CreditResponse> Create(CreateCreditRequest request);
    Task<IEnumerable<CreditResponse>> GetAll();
    Task<CreditResponse?> GetById(int creditId);
    Task<bool> Delete(int creditId);
}