using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs;
using CarCredit.Domain.Entities;

namespace CarCredit.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository _r)
    {
        _repository = _r;
    }

    public async Task<IEnumerable<Customer>> GetAll() => await _repository.GetAll();

    public async Task<Customer?> GetById(int customerId) => await _repository.GetById(customerId);

    public async Task<Customer> Create(CreateCustomerRequest request)
    {
        var customer = new Customer
        {
            Name = request.Name,
            Lastname = request.Lastname,
            Age = request.Age,
            Address = request.Address
        };

        await _repository.Add(customer);
        await _repository.SaveChanges();

        return customer;
    }

    public async Task<bool> Delete(int customerId)
    {
        var customer = await _repository.GetById(customerId);
        if (customer is null) return false;

        if (await _repository.HasUnpaidFees(customerId))
        {
            throw new InvalidOperationException(
                "The customer cannot be deleted because at least one of their loans has outstanding payments.");
        }

        await _repository.Remove(customer);
        await _repository.SaveChanges();

        return true;
    }
}