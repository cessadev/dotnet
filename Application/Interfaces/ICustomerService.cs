using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;

namespace CarCredit.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponse>> GetAll();
    Task<CustomerResponse?> GetById(int creditId);
    Task<CustomerResponse> Create(CreateCustomerRequest request);
    Task<bool> Delete(int creditId);
}