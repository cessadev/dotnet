using CarCredit.Models;

namespace CarCredit.Interfaces;

public interface ICreditService
{
    Task<IEnumerable<Credit>> GetAll();
    Task<Credit?> GetById(int creditId);
    Task<Credit> CreateWithFees(CreateCreditRequest request);
    Task<bool> Delete(int creditId);
}