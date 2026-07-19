using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface ILoanRepository
{
    Task<bool> CustomerExists(int customerId);
    Task<bool> HasUnpaidInstallments(int loanId);
    Task<IEnumerable<Loan>> GetAll();
    Task<Loan?> GetByReference(string reference);
    Task AddInstallments(IEnumerable<Installment> installments);
    Task Add(Loan loan);
    Task Remove(Loan loan);
    Task SaveChanges();
}