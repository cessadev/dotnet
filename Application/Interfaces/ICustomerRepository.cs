using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAll();
    Task<Customer?> GetById(int customerId);
    Task<bool> HasUnpaidFees(int customerId);
    Task Add(Customer customer);
    Task Remove(Customer customer);
    Task SaveChanges();
}