using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface ICreditRepository
{
    Task<bool> CustomerExists(int customerId);
    Task<bool> HasUnpaidFees(int creditId);
    Task<IEnumerable<Credit>> GetAll();
    Task<Credit?> GetById(int creditId);
    Task AddFees(IEnumerable<Fee> fees);
    Task Add(Credit credit);
    Task Remove(Credit credit);
    Task SaveChanges();
}