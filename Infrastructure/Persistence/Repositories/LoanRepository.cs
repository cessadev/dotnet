using Microsoft.EntityFrameworkCore;
using CarCredit.Application.Interfaces;
using CarCredit.Infrastructure.Persistence;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly AppDbContext _db;

    public LoanRepository(AppDbContext _database)
    {
        _db = _database;
    }

    public async Task<bool> HasUnpaidInstallments(string reference)
        => await _db.Installments.AnyAsync(i => i.Loan.Reference == reference && !i.Paid);

    public async Task<Loan?> GetByReference(string reference)
        => await _db.Loans
            .Include(l => l.Customer)
            .Include(l => l.Vehicle)
            .FirstOrDefaultAsync(l => l.Reference == reference);

    public Task AddInstallments(IEnumerable<Installment> installments)
    {
        _db.Installments.AddRange(installments);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Loan>> GetAll()
        => await _db.Loans
            .Include(l => l.Customer)
            .Include(l => l.Vehicle)
            .ToListAsync();

    public Task Add(Loan loan)
    {
        _db.Loans.Add(loan);
        return Task.CompletedTask;
    }

    public Task Remove(Loan loan)
    {
        _db.Loans.Remove(loan);
        return Task.CompletedTask;
    }
    
    public async Task SaveChanges() => await _db.SaveChangesAsync();
}