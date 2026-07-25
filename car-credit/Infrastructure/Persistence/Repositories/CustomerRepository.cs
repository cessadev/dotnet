using Microsoft.EntityFrameworkCore;
using CarCredit.Application.Interfaces;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _db;

    public CustomerRepository(AppDbContext database) => _db = database;

    public async Task<IEnumerable<Customer>> GetAll() => await _db.Customers.ToListAsync();

    public async Task<Customer?> GetByDocumentNumber(int documentNumber)
        => await _db.Customers.FirstOrDefaultAsync(c => c.DocumentNumber == documentNumber);

    public async Task<bool> HasUnpaidInstallments(int documentNumber)
        => await _db.Installments.AnyAsync(i =>
            !i.Paid && i.Loan.Customer.DocumentNumber == documentNumber);

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