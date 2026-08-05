using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;

namespace CarCredit.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponse>> GetAll();
    Task<CustomerResponse?> GetByDocumentNumber(int documentNumber);
    Task<CustomerResponse> Create(CreateCustomerRequest request);
    Task<CustomerResponse?> Update(int documentNumber, UpdateCustomerRequest request);
    Task<bool> Delete(int documentNumber);
}