using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAll();
    Task<Customer?> GetById(int creditId);
    Task<Customer> Create(CreateCustomerRequest request);
    Task<bool> Delete(int creditId);
}