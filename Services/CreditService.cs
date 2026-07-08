using CarCredit.Data;
using CarCredit.Interfaces;
using CarCredit.Models;
using Microsoft.EntityFrameworkCore;

namespace CarCredit.Services;

public class CreditService : ICreditService
{
    private readonly AppDbContext _db;

    public CreditService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Credit> CreateWithFees(CreateCreditRequest request)
    {
        if (request.Fee <= 0)
            throw new ArgumentException("The number of installments must be greater than zero.");

        var customerExists = await _db.Customers.AnyAsync(c => c.Id == request.CustomerId);
        if (!customerExists)
            throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var credit = new Credit
        {
            CustomerId = request.CustomerId,
            Vehicle = request.Vehicle,
            ValueCredit = request.ValueCredit,
            Fee = request.Fee
        };

        _db.Credits.Add(credit);
        await _db.SaveChangesAsync();

        decimal installment = Math.Floor((request.ValueCredit / request.Fee) * 100) / 100;
        decimal remaining = request.ValueCredit;

        var fees = new List<Fee>();

        for (int i = 1; i <= request.Fee; i++)
        {
            decimal value;

            if (i == request.Fee)
            {
                // The last installment receives the remaining funds
                value = remaining;
            }
            else
            {
                value = installment;
                remaining -= value;
            }

            fees.Add(new Fee
            {
               CreditId = credit.Id,
               NumberFee = i,
               ValueFee = value,
               DateExpiration = DateTime.UtcNow.AddMonths(i) 
            });
        }
        
        _db.Fees.AddRange(fees);
        await _db.SaveChangesAsync();

        return credit;
    }

    public async Task<bool> Delete(int creditId)
    {
        var credit = await _db.Credits.FindAsync(creditId);
        if (credit is null) return false;

        bool allPaid = await _db.Fees
            .Where(f => f.CreditId == creditId)
            .AllAsync(f => f.Paid == true);

        if (!allPaid)
            throw new InvalidOperationException("The credit cannot be deleted because it has unpaid fees.");

        _db.Credits.Remove(credit);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Credit>> GetAll() => await _db.Credits.Include(c => c.Customer).ToListAsync();

    public async Task<Credit?> GetById(int creditId) => await _db.Credits.Include(c => c.Customer).FirstOrDefaultAsync(c => c.Id == creditId);
}