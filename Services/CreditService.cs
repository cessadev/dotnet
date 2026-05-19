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
        var customerExists = await _db.Customers.AnyAsync(c => c.Id == request.CustomerId);
        if (!customerExists)
            throw new KeyNotFoundException($"Customer {request.CustomerId} not found");

        var credit = new Credit
        {
            CustomerId = request.CustomerId,
            Vehicle = request.Vehicle,
            ValueCredit = request.ValueCredit,
            Fee = request.Fee
        };

        _db.Credits.Add(credit);
        await _db.SaveChangesAsync();

        var feeValue = Math.Round(request.ValueCredit / request.Fee, 2);
        var fees = Enumerable.Range(1, request.Fee)
            .Select(i => new Fee
            {
                CreditId = credit.Id,
                NumberFee = i,
                ValueFee = feeValue,
                DateExpiration = DateTime.UtcNow.AddMonths(i)
            })
            .ToList();
        
        _db.Fees.AddRange(fees);
        await _db.SaveChangesAsync();

        return credit;
    }

    public async Task<bool> Delete(int creditId)
    {
        var credit = await _db.Credits.FindAsync(creditId);
        if (credit is null) return false;

        _db.Credits.Remove(credit);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<Credit>> GetAll() => await _db.Credits.Include(c => c.Customer).ToListAsync();

    public async Task<Credit?> GetById(int creditId) => await _db.Credits.Include(c => c.Customer).FirstOrDefaultAsync(c => c.Id == creditId);
}