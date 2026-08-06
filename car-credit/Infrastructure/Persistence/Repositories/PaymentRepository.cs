using Microsoft.EntityFrameworkCore;
using CarCredit.Application.Interfaces;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _db;

    public PaymentRepository(AppDbContext database)
    {
        _db = database;
    }

    public async Task<IEnumerable<Payment>> GetByInstallmentReference(string paymentReference)
        => await _db.Payments
            .Include(p => p.Installment)
                .ThenInclude(i => i.Loan)
            .Where(p => p.Installment.PaymentReference == paymentReference)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    
    public async Task<IEnumerable<Payment>> GetByLoanReference(string loanReference)
        => await _db.Payments
            .Include(p => p.Installment)
                .ThenInclude(i => i.Loan)
            .Where(p => p.Installment.Loan.Reference == loanReference)
            .OrderByDescending(p => p.Date)
            .ToListAsync();
}
