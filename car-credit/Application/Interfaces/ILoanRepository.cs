using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface ILoanRepository
{
    Task<bool> HasUnpaidInstallments(string reference);
    Task<Loan?> GetByReference(string reference);
    Task AddInstallments(IEnumerable<Installment> installments);
    Task<IEnumerable<Loan>> GetAll();
    Task Add(Loan loan);
    Task Remove(Loan loan);
    Task SaveChanges();
}