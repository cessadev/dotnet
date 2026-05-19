using CarCredit.Data;
using CarCredit.Interfaces;
using CarCredit.Models;
using Microsoft.EntityFrameworkCore;

namespace CarCredit.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;

    public CustomerService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Customer>> GetAll() => await _db.Customers.ToListAsync();

    public async Task<Customer?> GetById(int customerId) => await _db.Customers.FindAsync();

    public async Task<Customer> Create(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Lastname = request.Lastname,
            Age = request.Age,
            Address = request.Address
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        return customer;
    }

    public async Task<bool> Delete(int customerId)
    {
        var customer = await _db.Customers.FindAsync(customerId);
        if (customer is null) return false;

        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();

        return true;
    }
}