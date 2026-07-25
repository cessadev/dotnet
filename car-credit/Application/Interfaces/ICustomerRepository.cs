using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface ICustomerRepository
{
    Task<IEnumerable<Customer>> GetAll();
    Task<Customer?> GetByDocumentNumber(int documentNumber);
    Task<bool> HasUnpaidInstallments(int documentNumber);
    Task Add(Customer customer);
    Task Remove(Customer customer);
    Task SaveChanges();
}