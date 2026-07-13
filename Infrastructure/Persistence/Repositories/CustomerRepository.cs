using Microsoft.EntityFrameworkCore;
using CarCredit.Application.Interfaces;
using CarCredit.Infrastructure.Persistence;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext _database)
    {
        _db = _database;
    }

    public async Task<IEnumerable<Customer>> GetAll() => await _db.Customers.ToListAsync();
    
    public async Task<Customer?> GetById(int customerId) => await _db.Customers.FindAsync(customerId);
    
    public async Task<bool> HasUnpaidFees(int customerId)
    {
        return await _db.Fees
            .AnyAsync(f =>
                !f.Paid &&
                f.Credit.CustomerId == customerId);
    }

    public Task Add(Customer customer)
    {
        _db.Customers.Add(customer);
        return Task.CompletedTask;
    }

    public Task Remove(Customer customer)
    {
        _db.Customers.Remove(customer);
        return Task.CompletedTask;
    }

    public async Task SaveChanges() => await _db.SaveChangesAsync();
}