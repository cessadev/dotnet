using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface ICreditService
{
    Task<IEnumerable<Credit>> GetAll();
    Task<Credit?> GetById(int creditId);
    Task<Credit> Create(CreateCreditRequest request);
    Task<bool> Delete(int creditId);
}