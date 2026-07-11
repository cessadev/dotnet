using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface ICreditRepository
{
    Task<bool> CustomerExists(int customerId);
    Task AddCreditWithFees(Credit credit, IEnumerable<Fee> fees);
    Task<IEnumerable<Credit>> GetAll();
    Task<Credit?> GetById(int creditId);
    Task<Credit?> GetByIdWithFees(int creditId);
    Task Remove(Credit credit);
    Task SaveChanges();
}