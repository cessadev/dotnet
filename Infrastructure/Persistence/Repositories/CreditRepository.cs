using Microsoft.EntityFrameworkCore;
using CarCredit.Application.Interfaces;
using CarCredit.Infrastructure.Persistence;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Repositories;

public class CreditRepository : ICreditRepository
{
    private readonly AppDbContext _db;

    public CreditRepository(AppDbContext _database)
    {
        _db = _database;
    }

    public async Task<bool> CustomerExists(int customerId) => await _db.Customers.AnyAsync(c => c.Id == customerId);

    public async Task<bool> HasUnpaidFees(int creditId)
    {
        return await _db.Fees
            .AnyAsync(f => f.CreditId == creditId && !f.Paid);
    }

    public async Task<IEnumerable<Credit>> GetAll()
        => await _db.Credits
            .Include(c => c.Customer)
            .ToListAsync();

    public async Task<Credit?> GetById(int creditId) => await _db.Credits.FindAsync(creditId);

    public Task AddFees(IEnumerable<Fee> fees)
    {
        _db.Fees.AddRange(fees);
        return Task.CompletedTask;
    }

    public Task Add(Credit credit)
    {
        _db.Credits.Add(credit);
        return Task.CompletedTask;
    }

    public Task Remove(Credit credit)
    {
        _db.Credits.Remove(credit);
        return Task.CompletedTask;
    }
    
    public async Task SaveChanges() => await _db.SaveChangesAsync();
}